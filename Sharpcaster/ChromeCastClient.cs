using Extensions.Api.CastChannel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharpcaster.Channels;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
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

        private ILogger _logger = null;
        private TcpClient _client;
        private Stream _stream;
        private TaskCompletionSource<bool> ReceiveTcs { get; set; }
        private SemaphoreSlim SendSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        private IDictionary<string, Type> MessageTypes { get; set; }
        private IEnumerable<IChromecastChannel> Channels { get; set; }
        private ConcurrentDictionary<int, object> WaitingTasks { get; } = new ConcurrentDictionary<int, object>();

        public ChromecastClient(ILoggerFactory loggerFactory = null)
        {
            var serviceCollection = new ServiceCollection();
            
            if (loggerFactory != null) {
                serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);
                serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));  // see https://stackoverflow.com/questions/31751437/how-is-iloggert-resolved-via-di 
            }                                                                                            

            serviceCollection.AddTransient<IChromecastChannel, ConnectionChannel>();
            serviceCollection.AddTransient<IChromecastChannel, HeartbeatChannel>();
            serviceCollection.AddTransient<IChromecastChannel, ReceiverChannel>();
            serviceCollection.AddTransient<IChromecastChannel, MediaChannel>();
            serviceCollection.AddTransient<IChromecastChannel, MultiZoneChannel>();
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

            MessageTypes = messages.Where(t => !string.IsNullOrEmpty(t.Type)).ToDictionary(m => m.Type, m => m.GetType());
            Channels = channels;

            _logger = serviceProvider.GetService<ILogger<ChromecastClient>>();
            _logger?.LogDebug(MessageTypes.Keys.ToString(","));
            _logger?.LogDebug(Channels.ToString(","));

            foreach (var channel in Channels)
            {
                channel.Client = this;
            }
        }

        public async Task<ChromecastStatus> ConnectChromecast(ChromecastReceiver chromecastReceiver)
        {
            await Dispose();

            _client = new TcpClient();
            await _client.ConnectAsync(chromecastReceiver.DeviceUri.Host, chromecastReceiver.Port);
            
            //Open SSL stream to Chromecast and bypass all SSL validation
            var secureStream = new SslStream(_client.GetStream(), true, (sender, certificate, chain, sslPolicyErrors) => true);
            await secureStream.AuthenticateAsClientAsync(chromecastReceiver.DeviceUri.Host);
            _stream = secureStream;
            

            ReceiveTcs = new TaskCompletionSource<bool>();
            Receive();
            GetChannel<IHeartbeatChannel>().StartTimeoutTimer();
            GetChannel<IHeartbeatChannel>().StatusChanged += HeartBeatTimedOut;
            await GetChannel<IConnectionChannel>().ConnectAsync();
            return await GetChannel<IReceiverChannel>().GetChromecastStatusAsync();
        }

        private async void HeartBeatTimedOut(object sender, EventArgs e)
        {
            _logger?.LogError("Heartbeat timeout - Disconnecting client.");
            await DisconnectAsync();
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

                        var channel = Channels.FirstOrDefault(c => c.Namespace == castMessage.Namespace);
                        if (channel != null)
                        {
                            if (channel != GetChannel<IHeartbeatChannel>())
                            {
                                GetChannel<IHeartbeatChannel>().StopTimeoutTimer();
                            }
                            channel?._logger?.LogTrace($"RECEIVED: {payload}");
                            
                            var message = JsonConvert.DeserializeObject<MessageWithId>(payload);
                            if (MessageTypes.TryGetValue(message.Type, out Type type))
                            {
                                object tcs = null;
                                try
                                {
                                    var response = (IMessage)JsonConvert.DeserializeObject(payload, type);
                                    await channel.OnMessageReceivedAsync(response);
                                    TaskCompletionSourceInvoke(ref tcs, message, "SetResult", response);
                                }
                                catch (Exception ex)
                                {
                                    _logger?.LogError($"Exception processing the Response: {ex.Message}");
                                    TaskCompletionSourceInvoke(ref tcs, message, "SetException", ex, new Type[] { typeof(Exception) });
                                }
                            }
                            else
                            {
                                _logger?.LogError("The received Message of Type '{ty}' can not be converted to its response Type." +
                                   " An implementing IMessage class is missing!", message.Type);
                                Debugger.Break();
                            }
                        } else
                        {
                            _logger?.LogError($"Couldn't parse the channel from: {castMessage.Namespace}  :  {payload}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger?.LogError($"Error in receive loop: {exception.Message}");
                    //await Dispose(false);
                    ReceiveTcs.SetResult(true);
                }
            });
        }

        private void TaskCompletionSourceInvoke(ref object tcs, MessageWithId message, string method, object parameter, Type[] types = null)
        {
            if (tcs == null) {
                if (message.HasRequestId && WaitingTasks.TryRemove(message.RequestId, out object newtcs)) {
                    tcs = newtcs;
                }
            }
            if (tcs != null) {
                var tcsType = tcs.GetType();
                (types == null ? tcsType.GetMethod(method) : tcsType.GetMethod(method, types)).Invoke(tcs, new object[] { parameter });
            }
        }

        public async Task SendAsync(ILogger channelLogger, string ns, IMessage message, string destinationId)
        {
            var castMessage = CreateCastMessage(ns, destinationId);
            castMessage.PayloadUtf8 = JsonConvert.SerializeObject(message);
            await SendAsync(channelLogger, castMessage);
        }

        private async Task SendAsync(ILogger channelLogger, CastMessage castMessage)
        {
            await SendSemaphoreSlim.WaitAsync();
            try
            {
                ((channelLogger!=null)?channelLogger:_logger)?.LogTrace($"SENT    : {castMessage.DestinationId}: {castMessage.PayloadUtf8}");
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

        public async Task<TResponse> SendAsync<TResponse>(ILogger channelLogger, string ns, IMessageWithId message, string destinationId) where TResponse : IMessageWithId
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            WaitingTasks[message.RequestId] = taskCompletionSource;
            await SendAsync(channelLogger, ns, message, destinationId);
            return await taskCompletionSource.Task.TimeoutAfter(RECEIVE_TIMEOUT);
        }

        public async Task DisconnectAsync()
        {
            foreach (var channel in GetStatusChannels())
            {
                channel.GetType().GetProperty("Status").SetValue(channel, null);
            }
            GetChannel<IHeartbeatChannel>().StopTimeoutTimer();
            GetChannel<IHeartbeatChannel>().StatusChanged -= HeartBeatTimedOut;
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
                    _logger?.LogError($"Error on disposing. {ex.Message}");
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
                } else {
                    // another AppId is running
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