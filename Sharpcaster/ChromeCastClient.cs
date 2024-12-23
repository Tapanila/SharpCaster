using Extensions.Api.CastChannel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharpcaster.Channels;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Messages.Heartbeat;
using Sharpcaster.Messages.Media;
using Sharpcaster.Messages.Multizone;
using Sharpcaster.Messages.Queue;
using Sharpcaster.Messages.Receiver;
using Sharpcaster.Messages.Spotify;
using Sharpcaster.Models;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Media;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
        public Guid SenderId { get; } = Guid.NewGuid();
        public string FriendlyName { get; set; }

        public IMediaChannel MediaChannel => GetChannel<IMediaChannel>();
        public IHeartbeatChannel HeartbeatChannel => GetChannel<IHeartbeatChannel>();
        public IReceiverChannel ReceiverChannel => GetChannel<IReceiverChannel>();
        public IConnectionChannel ConnectionChannel => GetChannel<IConnectionChannel>();
        public MultiZoneChannel MultiZoneChannel => GetChannel<MultiZoneChannel>();

        private ILogger _logger = null;
        private TcpClient _client;
        private SslStream _stream;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> ReceiveTcs { get; set; }
        private SemaphoreSlim SendSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);
        private JsonSerializerOptions _jsonSerializerOptions;
        private Dictionary<string, Type> MessageTypes { get; set; }
        private IEnumerable<IChromecastChannel> Channels { get; set; }
        private ConcurrentDictionary<int, SharpCasterTaskCompletionSource> WaitingTasks { get; } = new ConcurrentDictionary<int, SharpCasterTaskCompletionSource>();

        public ChromecastClient(ILoggerFactory loggerFactory = null)
        {
            var serviceCollection = new ServiceCollection();

            if (loggerFactory != null)
            {
                serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);
                serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));  // see https://stackoverflow.com/questions/31751437/how-is-iloggert-resolved-via-di 
            }

            serviceCollection.AddTransient<IChromecastChannel, ConnectionChannel>();
            serviceCollection.AddTransient<IChromecastChannel, HeartbeatChannel>();
            serviceCollection.AddTransient<IChromecastChannel, ReceiverChannel>();
            serviceCollection.AddTransient<IChromecastChannel, MediaChannel>();
            serviceCollection.AddTransient<IChromecastChannel, MultiZoneChannel>();
            serviceCollection.AddTransient<IChromecastChannel, SpotifyChannel>();
            var messageInterfaceType = typeof(IMessage);
            serviceCollection.AddTransient(messageInterfaceType, typeof(AddUserResponseMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(GetInfoResponseMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(LaunchErrorMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(ReceiverStatusMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(QueueChangeMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(QueueItemIdsMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(QueueItemsMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(DeviceUpdatedMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(MultizoneStatusMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(ErrorMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(InvalidRequestMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(LoadCancelledMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(LoadFailedMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(MediaStatusMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(PingMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(CloseMessage));
            serviceCollection.AddTransient(messageInterfaceType, typeof(LaunchStatusMessage));

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
            _logger?.LogDebug("MessageTypes: {MessageTypes}", MessageTypes.Keys.ToString(","));
            _logger?.LogDebug("Channels: {Channels}", Channels.ToString(","));

            foreach (var channel in Channels)
            {
                channel.Client = this;
            }

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = SharpcasteSerializationContext.Default
            };
        }

        public async Task<ChromecastStatus> ConnectChromecast(ChromecastReceiver chromecastReceiver)
        {
            if (chromecastReceiver.DeviceUri == null)
            {
                throw new ArgumentNullException(nameof(chromecastReceiver.DeviceUri));
            }
            await Dispose();
            FriendlyName = chromecastReceiver.Name;

            _client = new TcpClient();
            await _client.ConnectAsync(chromecastReceiver.DeviceUri.Host, chromecastReceiver.Port);

            //Open SSL stream to Chromecast and bypass all SSL validation
            var secureStream = new SslStream(_client.GetStream(), true, (_, __, ___, ____) => true);
            await secureStream.AuthenticateAsClientAsync(chromecastReceiver.DeviceUri.Host);
            _stream = secureStream;

            _cancellationTokenSource = new CancellationTokenSource();
            ReceiveTcs = new TaskCompletionSource<bool>();
            Receive(_cancellationTokenSource.Token);
            HeartbeatChannel.StartTimeoutTimer();
            HeartbeatChannel.StatusChanged += HeartBeatTimedOut;
            await ConnectionChannel.ConnectAsync();
            return await ReceiverChannel.GetChromecastStatusAsync();
        }

        private async void HeartBeatTimedOut(object sender, EventArgs e)
        {
            _logger?.LogError("Heartbeat timeout - Disconnecting client.");
            await DisconnectAsync();
        }

        private void Receive(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        //First 4 bytes contains the length of the message
                        var buffer = await _stream.ReadAsync(4, cancellationToken);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(buffer);
                        }
                        var length = BitConverter.ToInt32(buffer, 0);
                        var castMessage = CastMessage.Parser.ParseFrom(await _stream.ReadAsync(length, cancellationToken));
                        //Payload can either be Binary or UTF8 json
                        var payload = (castMessage.PayloadType == PayloadType.Binary ?
                            Encoding.UTF8.GetString(castMessage.PayloadBinary.ToByteArray()) : castMessage.PayloadUtf8);

                        var channel = Channels.FirstOrDefault(c => c.Namespace == castMessage.Namespace);
                        if (channel != null)
                        {
                            if (channel != HeartbeatChannel)
                            {
                                HeartbeatChannel.StopTimeoutTimer();
                            }
                            channel?.Logger?.LogTrace("RECEIVED: {payload}", payload);

                            var message = JsonSerializer.Deserialize(payload, SharpcasteSerializationContext.Default.MessageWithId);
                            if (MessageTypes.TryGetValue(message.Type, out Type type))
                            {
                                try
                                {
                                    await channel.OnMessageReceivedAsync(payload, message.Type);
                                    if (message.HasRequestId)
                                    {
                                        WaitingTasks.TryRemove(message.RequestId, out SharpCasterTaskCompletionSource tcs);
                                        tcs?.SetResult(payload);
                                        if (tcs == null)
                                            _logger.LogTrace("No TaskCompletionSource found for RequestId: {RequestId}, CompletionSourceCount: {CompletionSourceCount}, Type: {Type} ", message.RequestId, WaitingTasks.Count, message.Type);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger?.LogError("Exception processing the Response: {Message}", ex.Message);
                                    if (message.HasRequestId)
                                    {
                                        WaitingTasks.TryRemove(message.RequestId, out SharpCasterTaskCompletionSource tcs);
                                        tcs?.SetException(ex);
                                    }
                                }
                            }
                            else
                            {
                                _logger?.LogError("The received Message of Type '{ty}' can not be converted to its response Type." +
                                   " An implementing IMessage class is missing!", message.Type);
                                Debugger.Break();
                            }
                        }
                        else
                        {
                            _logger?.LogError("Couldn't parse the channel from: {NameSpace} : {Payload}", castMessage.Namespace, payload);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger?.LogError("Error in receive loop: {Message}", exception.Message);
                    //await Dispose(false);
                    ReceiveTcs.SetResult(true);
                }
            }, cancellationToken);
        }

        public async Task SendAsync(ILogger channelLogger, string ns, string messagePayload, string destinationId)
        {
            var castMessage = CreateCastMessage(ns, destinationId);
            castMessage.PayloadUtf8 = messagePayload;
            await SendAsync(channelLogger, castMessage);
        }

        private async Task SendAsync(ILogger channelLogger, CastMessage castMessage)
        {
            await SendSemaphoreSlim.WaitAsync();
            try
            {
                (channelLogger ?? _logger)?.LogTrace("SENT: {NameSpace} - {DestinationId}: {PayloadUtf8}", castMessage.Namespace, castMessage.DestinationId, castMessage.PayloadUtf8);
#if NETSTANDARD2_0
                byte[] message = castMessage.ToProto();
#else
                ReadOnlyMemory<byte> message = castMessage.ToProto();
#endif
#if NETSTANDARD2_0
                await _stream.WriteAsync(message, 0, message.Length);
#else
                await _stream.WriteAsync(message);
#endif
                await _stream.FlushAsync();
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
                SourceId = SenderId.ToString(),
                DestinationId = destinationId
            };
        }

        public async Task<string> SendAsync(ILogger channelLogger, string ns, int messageRequestId, string messagePayload, string destinationId)
        {
            var taskCompletionSource = new SharpCasterTaskCompletionSource();
            WaitingTasks[messageRequestId] = taskCompletionSource;
            await SendAsync(channelLogger, ns, messagePayload, destinationId);
            return await taskCompletionSource.Task.TimeoutAfter(RECEIVE_TIMEOUT);
        }

        public async Task<string> WaitResponseAsync(int messageRequestId)
        {
            var taskCompletionSource = new SharpCasterTaskCompletionSource();
            WaitingTasks[messageRequestId] = taskCompletionSource;
            return await taskCompletionSource.Task.TimeoutAfter(RECEIVE_TIMEOUT);
        }

        public async Task DisconnectAsync()
        {
            foreach (var channel in GetStatusChannels())
            {
                channel.ClearStatus();
            }
            HeartbeatChannel.StopTimeoutTimer();
            HeartbeatChannel.StatusChanged -= HeartBeatTimedOut;
            _cancellationTokenSource.Cancel(true);
            await Task.Delay(100);
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
                    _logger?.LogError("Error on disposing. {Message}", ex.Message);
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
                    await ConnectionChannel.ConnectAsync(runningApplication.TransportId);
                    //Check if the application is using the media namespace
                    //If so go and get the media status
                    if (runningApplication.Namespaces.Where(ns => ns.Name == "urn:x-cast:com.google.cast.media") != null)
                    {
                        await MediaChannel.GetMediaStatusAsync();
                    }
                    return await ReceiverChannel.GetChromecastStatusAsync();
                }
            }
            var newApplication = await ReceiverChannel.LaunchApplicationAsync(applicationId);
            await ConnectionChannel.ConnectAsync(newApplication.Application.TransportId);
            return await ReceiverChannel.GetChromecastStatusAsync();
        }

        private IEnumerable<IStatusChannel<object>> GetStatusChannels()
        {
            return Channels.OfType<IStatusChannel<object>>();
        }

        /// <summary>
        /// Gets the differents statuses
        /// </summary>
        /// <returns>a dictionnary of namespace/status</returns>
        public IDictionary<string, object> GetStatuses()
        {
            return GetStatusChannels().ToDictionary(c => c.Namespace, c => c.Status);
        }

        public ChromecastStatus GetChromecastStatus()
        {
            return ReceiverChannel.Status;
        }

        public MediaStatus GetMediaStatus()
        {
            return MediaChannel.MediaStatus;
        }
    }
}