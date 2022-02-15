using Extensions.Api.CastChannel;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sharpcaster.Channels;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Media;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Extensions.Api.CastChannel.CastMessage.Types;

namespace Sharpcaster
{
    public class ChromecastClient : IChromecastClient
    {
        private const int RECEIVE_TIMEOUT = 30000;

        /// <summary>
        /// Raised when the sender is disconnected
        /// </summary>
        public event EventHandler Disconnected;

        private static readonly object LockObject = new object();
        private TcpClient _client;
        private Stream _stream;
        private ChromecastReceiver _receiver;
        private TaskCompletionSource<bool> ReceiveTcs { get; set; }
        private SemaphoreSlim SendSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        private IDictionary<string, Type> MessageTypes { get; set; }
        private IEnumerable<IChromecastChannel> Channels { get; set; }
        private ConcurrentDictionary<int, object> WaitingTasks { get; } = new ConcurrentDictionary<int, object>();

        public ChromecastClient()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IChromecastChannel, ConnectionChannel>();
            serviceCollection.AddTransient<IChromecastChannel, HeartbeatChannel>();
            serviceCollection.AddTransient<IChromecastChannel, ReceiverChannel>();
            serviceCollection.AddTransient<IChromecastChannel, MediaChannel>();
            var messageInterfaceType = typeof(IMessage);
            foreach (var type in (from t in typeof(IConnectionChannel).GetTypeInfo().Assembly.GetTypes()
                                  where t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && messageInterfaceType.IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ReceptionMessageAttribute>() != null
                                  select t))
            {
                serviceCollection.AddTransient(messageInterfaceType, type);
            }
            Init(serviceCollection);
        }

        /// <summary>
        /// Initializes a new instance of Sender class
        /// </summary>
        /// <param name="serviceCollection">collection of service descriptors</param>
        public ChromecastClient(IServiceCollection serviceCollection)
        {
            Init(serviceCollection);
        }

        private void Init(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var channels = serviceProvider.GetServices<IChromecastChannel>();
            var messages = serviceProvider.GetServices<IMessage>();
            MessageTypes = messages.Where(t => !String.IsNullOrEmpty(t.Type)).ToDictionary(m => m.Type, m => m.GetType());

            Console.WriteLine(MessageTypes.Keys.ToString(","));
            Channels = channels;
            Console.WriteLine(Channels.ToString(","));
            foreach (var channel in channels)
            {
                channel.Client = this;
            }
        }


        public async Task<ChromecastStatus> ConnectChromecast(ChromecastReceiver chromecastReceiver)
        {
            await Dispose();

            _receiver = chromecastReceiver;
            _client = new TcpClient();
            await _client.ConnectAsync(chromecastReceiver.DeviceUri.Host, chromecastReceiver.Port);
            //Open SSL stream to Chromecast and bypass all SSL validation
            var secureStream = new SslStream(_client.GetStream(), true, (sender, certificate, chain, sslPolicyErrors) => true);
            await secureStream.AuthenticateAsClientAsync(chromecastReceiver.DeviceUri.Host);
            _stream = secureStream;

            ReceiveTcs = new TaskCompletionSource<bool>();
            Receive();
            await GetChannel<IConnectionChannel>().ConnectAsync();
            return await GetChannel<IReceiverChannel>().GetChromecastStatusAsync();
        }

        private void Receive()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        //First 4 bytes contains the length of the message
                        var buffer = await _stream.ReadAsync(4);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(buffer);
                        }
                        var length = BitConverter.ToInt32(buffer, 0);
                        var castMessage = CastMessage.Parser.ParseFrom(await _stream.ReadAsync(length));
                        //Payload can either be Binary or UTF8 json
                        var payload = (castMessage.PayloadType == PayloadType.Binary ?
                            Encoding.UTF8.GetString(castMessage.PayloadBinary.ToByteArray()) : castMessage.PayloadUtf8);
                        Console.WriteLine($"RECEIVED: {castMessage.Namespace} : {payload}");

                        var channel = Channels.FirstOrDefault(c => c.Namespace == castMessage.Namespace);
                        if (channel != null)
                        {
                            var message = JsonConvert.DeserializeObject<MessageWithId>(payload);
                            if (MessageTypes.TryGetValue(message.Type, out Type type))
                            {
                                try
                                {
                                    var response = (IMessage)JsonConvert.DeserializeObject(payload, type);
                                    await channel.OnMessageReceivedAsync(response);
                                    TaskCompletionSourceInvoke(message, "SetResult", response);
                                }
                                catch (Exception ex)
                                {
                                    TaskCompletionSourceInvoke(message, "SetException", ex, new Type[] { typeof(Exception) });
                                }
                            }
                            else
                            {
                                Debugger.Break();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //await Dispose(false);
                    ReceiveTcs.SetResult(true);
                }
            });
        }

        private async void TaskCompletionSourceInvoke(MessageWithId message, string method, object parameter, Type[] types = null)
        {
            if (message.HasRequestId && WaitingTasks.TryRemove(message.RequestId, out object tcs))
            {
                var tcsType = tcs.GetType();
                (types == null ? tcsType.GetMethod(method) : tcsType.GetMethod(method, types)).Invoke(tcs, new object[] { parameter });
            }
            else
            {
                //This is just to handle media status messages. Where we want to update the status of media but we are not expecting an update
                if (message.Type == "MEDIA_STATUS")
                {
                    var statusMessage = parameter as MediaStatusMessage;
                    await GetChannel<MediaChannel>().OnMessageReceivedAsync(statusMessage);
                }
            }
        }

        public async Task SendAsync(string ns, IMessage message, string destinationId)
        {
            var castMessage = CreateCastMessage(ns, destinationId);
            castMessage.PayloadUtf8 = JsonConvert.SerializeObject(message);
            await SendAsync(castMessage);
        }

        private async Task SendAsync(CastMessage castMessage)
        {
            await SendSemaphoreSlim.WaitAsync();
            try
            {
                Console.WriteLine($"SENT    : {castMessage.DestinationId}: {castMessage.PayloadUtf8}");

                byte[] message = castMessage.ToProto();
                var networkStream = _stream;
                await networkStream.WriteAsync(message, 0, message.Length);
                await networkStream.FlushAsync();
            }
            finally
            {
                SendSemaphoreSlim.Release();
            }
        }

        private CastMessage CreateCastMessage(string ns, string destinationId)
        {
            return new CastMessage()
            {
                Namespace = ns,
                SourceId = "Sender-0",
                DestinationId = destinationId
            };
        }

        public async Task<TResponse> SendAsync<TResponse>(string ns, IMessageWithId message, string destinationId) where TResponse : IMessageWithId
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            WaitingTasks[message.RequestId] = taskCompletionSource;
            await SendAsync(ns, message, destinationId);
            return await taskCompletionSource.Task.TimeoutAfter(RECEIVE_TIMEOUT);
        }

        public async Task DisconnectAsync()
        {
            foreach (var channel in GetStatusChannels())
            {
                channel.GetType().GetProperty("Status").SetValue(channel, null);
            }
            await Dispose();
        }


        private async Task Dispose()
        {
            await Dispose(true);
        }

        private async Task Dispose(bool waitReceiveTask)
        {
            if (_client != null)
            {
                WaitingTasks.Clear();
                Dispose(_stream, () => _stream = null);
                Dispose(_client, () => _client = null);
                if (waitReceiveTask && ReceiveTcs != null)
                {
                    await ReceiveTcs.Task;
                }
                OnDisconnected();
            }
        }

        private void Dispose(IDisposable disposable, Action action)
        {
            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on disposing.", ex, null);
                }
                finally
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Raises the Disconnected event
        /// </summary>
        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets a channel
        /// </summary>
        /// <typeparam name="TChannel">channel type</typeparam>
        /// <returns>a channel</returns>
        public TChannel GetChannel<TChannel>() where TChannel : IChromecastChannel
        {
            return Channels.OfType<TChannel>().FirstOrDefault();
        }

        public async Task<ChromecastStatus> LaunchApplicationAsync(string applicationId, bool joinExistingApplicationSession = true)
        {

            if (joinExistingApplicationSession)
            {
                var status = GetChromecastStatus();
                var runningApplication = status?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
                if (runningApplication != null)
                {
                    await GetChannel<IConnectionChannel>().ConnectAsync(runningApplication.TransportId);
                    return await GetChannel<IReceiverChannel>().GetChromecastStatusAsync();
                }
            }
            var newApplication = await GetChannel<IReceiverChannel>().LaunchApplicationAsync(applicationId);
            await GetChannel<IConnectionChannel>().ConnectAsync(newApplication.Applications.First().TransportId);
            return await GetChannel<IReceiverChannel>().GetChromecastStatusAsync();
        }

        private IEnumerable<IChromecastChannel> GetStatusChannels()
        {
            var statusChannelType = typeof(IStatusChannel<>);
            return Channels.Where(c => c.GetType().GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == statusChannelType));
        }

        /// <summary>
        /// Gets the differents statuses
        /// </summary>
        /// <returns>a dictionnary of namespace/status</returns>
        public IDictionary<string, object> GetStatuses()
        {
            return GetStatusChannels().ToDictionary(c => c.Namespace, c => c.GetType().GetProperty("Status").GetValue(c));
        }

        public ChromecastStatus GetChromecastStatus()
        {
            return GetStatuses().First(x => x.Key == GetChannel<ReceiverChannel>().Namespace).Value as ChromecastStatus;
        }

        public MediaStatus GetMediaStatus()
        {
            return GetStatuses().First(x => x.Key == GetChannel<MediaChannel>().Namespace).Value as MediaStatus;
        }
    }
}