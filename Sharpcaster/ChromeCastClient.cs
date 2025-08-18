using Extensions.Api.CastChannel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharpcaster.Channels;
using Sharpcaster.Extensions;
using System.Runtime.CompilerServices;
using Sharpcaster.Interfaces;
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
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Extensions.Api.CastChannel.CastMessage.Types;

namespace Sharpcaster
{
    public class ChromecastClient
    {
        private const int RECEIVE_TIMEOUT = 30000;

        /// <summary>
        /// Raised when the sender is disconnected
        /// </summary>
        public event EventHandler? Disconnected;
        public Guid SenderId { get; } = Guid.NewGuid();
        public string FriendlyName { get; set; } = string.Empty;

        public MediaChannel MediaChannel => GetChannel<MediaChannel>();
        public HeartbeatChannel HeartbeatChannel => GetChannel<HeartbeatChannel>();
        public ReceiverChannel ReceiverChannel => GetChannel<ReceiverChannel>();
        public ConnectionChannel ConnectionChannel => GetChannel<ConnectionChannel>();
        public MultiZoneChannel MultiZoneChannel => GetChannel<MultiZoneChannel>();

        private ILogger? _logger;

        private static readonly Action<ILogger, string, Exception?> LogMessageTypes =
            LoggerMessage.Define<string>(LogLevel.Debug, new EventId(4001, "MessageTypes"), "MessageTypes: {MessageTypes}");

        private static readonly Action<ILogger, string, Exception?> LogChannels =
            LoggerMessage.Define<string>(LogLevel.Debug, new EventId(4002, "Channels"), "Channels: {Channels}");

        private static readonly Action<ILogger, Exception?> LogHeartbeatTimeout =
            LoggerMessage.Define(LogLevel.Error, new EventId(4003, "HeartbeatTimeout"), "Heartbeat timeout - Disconnecting client.");

        private static readonly Action<ILogger, string, Exception?> LogReceivedMessage =
            LoggerMessage.Define<string>(LogLevel.Trace, new EventId(4004, "ReceivedMessage"), "RECEIVED: {Payload}");

        private static readonly Action<ILogger, int, int, string, Exception?> LogNoTaskCompletionSource =
            LoggerMessage.Define<int, int, string>(LogLevel.Trace, new EventId(4005, "NoTaskCompletionSource"), "No TaskCompletionSource found for RequestId: {RequestId}, CompletionSourceCount: {CompletionSourceCount}, Type: {Type} ");

        private static readonly Action<ILogger, string, Exception?> LogExceptionProcessingResponse =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(4006, "ExceptionProcessingResponse"), "Exception processing the Response: {Message}");

        private static readonly Action<ILogger, string, Exception?> LogMessageConversionError =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(4007, "MessageConversionError"), "The received Message of Type '{Ty}' can not be converted to its response Type. Please add it to the registered Services in Dependency Injection");

        private static readonly Action<ILogger, string, string, Exception?> LogChannelParseError =
            LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(4008, "ChannelParseError"), "Couldn't parse the channel from: {NameSpace} : {Payload}");

        private static readonly Action<ILogger, string, Exception?> LogReceiveLoopError =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(4009, "ReceiveLoopError"), "Error in receive loop: {Message}");

        private static readonly Action<ILogger, string, string, string, Exception?> LogSentMessage =
            LoggerMessage.Define<string, string, string>(LogLevel.Trace, new EventId(4010, "SentMessage"), "SENT: {NameSpace} - {DestinationId}: {PayloadUtf8}");

        private static readonly Action<ILogger, string, Exception?> LogDisposeError =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(4011, "DisposeError"), "Error on disposing. {Message}");
        private TcpClient? _client;
        private SslStream? _stream;
        private CancellationTokenSource _cancellationTokenSource = new();
        private TaskCompletionSource<bool> ReceiveTcs { get; set; } = new();
        private SemaphoreSlim SendSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);
        private JsonSerializerOptions _jsonSerializerOptions = new();
        private Dictionary<string, Type> MessageTypes { get; set; } = new();
        private IEnumerable<IChromecastChannel> Channels { get; set; } = new List<IChromecastChannel>();
        private ConcurrentDictionary<int, SharpCasterTaskCompletionSource> WaitingTasks { get; } = new ConcurrentDictionary<int, SharpCasterTaskCompletionSource>();
        private IServiceProvider _serviceProvider = null!;

        public ChromecastClient() : this(null)
        {
        }

        public ChromecastClient(ILogger<ChromecastClient>? logger)
        {
            _logger = logger;

            var serviceCollection = new ServiceCollection();
            RegisterChannels(serviceCollection, logger);
            RegisterMessages(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            InitializeClient();
        }

        private static void RegisterChannels(IServiceCollection services, ILogger<ChromecastClient>? logger)
        {
            // Create channel-specific loggers using the main logger as a base
            var connectionLogger = logger != null ? new ChannelLogger<ConnectionChannel>(logger) : null;
            var heartbeatLogger = logger != null ? new ChannelLogger<HeartbeatChannel>(logger) : null;
            var receiverLogger = logger != null ? new ChannelLogger<ReceiverChannel>(logger) : null;
            var mediaLogger = logger != null ? new ChannelLogger<MediaChannel>(logger) : null;
            var multizoneLogger = logger != null ? new ChannelLogger<MultiZoneChannel>(logger) : null;
            var spotifyLogger = logger != null ? new ChannelLogger<SpotifyChannel>(logger) : null;

            services.AddTransient<IChromecastChannel>(_ => new ConnectionChannel(connectionLogger));
            services.AddTransient<IChromecastChannel>(_ => new HeartbeatChannel(heartbeatLogger));
            services.AddTransient<IChromecastChannel>(_ => new ReceiverChannel(receiverLogger));
            services.AddTransient<IChromecastChannel>(_ => new MediaChannel(mediaLogger));
            services.AddTransient<IChromecastChannel>(_ => new MultiZoneChannel(multizoneLogger));
            services.AddTransient<IChromecastChannel>(_ => new SpotifyChannel(spotifyLogger));
        }

        private static void RegisterMessages(IServiceCollection services)
        {
            var messageInterfaceType = typeof(IMessage);
            services.AddTransient(messageInterfaceType, typeof(AddUserResponseMessage));
            services.AddTransient(messageInterfaceType, typeof(GetInfoResponseMessage));
            services.AddTransient(messageInterfaceType, typeof(LaunchErrorMessage));
            services.AddTransient(messageInterfaceType, typeof(ReceiverStatusMessage));
            services.AddTransient(messageInterfaceType, typeof(QueueChangeMessage));
            services.AddTransient(messageInterfaceType, typeof(QueueItemIdsMessage));
            services.AddTransient(messageInterfaceType, typeof(QueueItemsMessage));
            services.AddTransient(messageInterfaceType, typeof(DeviceUpdatedMessage));
            services.AddTransient(messageInterfaceType, typeof(MultizoneStatusMessage));
            services.AddTransient(messageInterfaceType, typeof(ErrorMessage));
            services.AddTransient(messageInterfaceType, typeof(InvalidRequestMessage));
            services.AddTransient(messageInterfaceType, typeof(LoadCancelledMessage));
            services.AddTransient(messageInterfaceType, typeof(LoadFailedMessage));
            services.AddTransient(messageInterfaceType, typeof(MediaStatusMessage));
            services.AddTransient(messageInterfaceType, typeof(PingMessage));
            services.AddTransient(messageInterfaceType, typeof(CloseMessage));
            services.AddTransient(messageInterfaceType, typeof(LaunchStatusMessage));
        }

        private void InitializeClient()
        {
            var channels = _serviceProvider.GetServices<IChromecastChannel>();
            var messages = _serviceProvider.GetServices<IMessage>();

            MessageTypes = messages.Where(t => !string.IsNullOrEmpty(t.Type)).ToDictionary(m => m.Type, m => m.GetType());
            Channels = channels;

            if (_logger != null) LogMessageTypes(_logger, $"[{string.Join(",", MessageTypes.Keys)}]", null);
            if (_logger != null) LogChannels(_logger, string.Join(",", Channels.Select(c => c.GetType().Name)), null);

            foreach (var channel in Channels)
            {
                channel.Client = this;
            }

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = SharpcasteSerializationContext.Default
            };
        }


        public async Task<ChromecastStatus?> ConnectChromecast(ChromecastReceiver chromecastReceiver)
        {
            if (chromecastReceiver?.DeviceUri == null)
            {
                throw new ArgumentNullException(nameof(chromecastReceiver));
            }
            await Dispose().ConfigureAwait(false);
            FriendlyName = chromecastReceiver.Name;

            _client = new TcpClient();
            await _client.ConnectAsync(chromecastReceiver.DeviceUri.Host, chromecastReceiver.Port).ConfigureAwait(false);

            //Open SSL stream to Chromecast and bypass all SSL validation
#pragma warning disable CA5359 // Do Not Disable Certificate Validation
            var secureStream = new SslStream(_client.GetStream(), true, (_, __, ___, ____) => true);
#pragma warning restore CA5359 // Do Not Disable Certificate Validation
            await secureStream.AuthenticateAsClientAsync(chromecastReceiver.DeviceUri.Host).ConfigureAwait(false);
            _stream = secureStream;

            _cancellationTokenSource = new CancellationTokenSource();
            ReceiveTcs = new TaskCompletionSource<bool>();
            Receive(_cancellationTokenSource.Token);
            HeartbeatChannel.StartTimeoutTimer();
            HeartbeatChannel.StatusChanged += HeartBeatTimedOut;
            await ConnectionChannel.ConnectAsync().ConfigureAwait(false);
            return await ReceiverChannel.GetChromecastStatusAsync().ConfigureAwait(false);
        }

        private async void HeartBeatTimedOut(object sender, EventArgs e)
        {
            if (_logger != null) LogHeartbeatTimeout(_logger, null);
            await DisconnectAsync().ConfigureAwait(false);
        }

        private void Receive(CancellationToken cancellationToken) => Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    //First 4 bytes contains the length of the message
                    var buffer = await _stream!.ReadAsync(4, cancellationToken).ConfigureAwait(false);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(buffer);
                    }
                    var length = BitConverter.ToInt32(buffer, 0);
                    var castMessage = CastMessage.Parser.ParseFrom(await _stream.ReadAsync(length, cancellationToken).ConfigureAwait(false));
                    //Payload can either be Binary or UTF8 json
                    var payload = (castMessage.PayloadType == PayloadType.Binary ?
                        Encoding.UTF8.GetString(castMessage.PayloadBinary.ToByteArray()) : castMessage.PayloadUtf8);

                    var channel = Channels.FirstOrDefault(c => c.Namespace == castMessage.Namespace);
                    if (channel != null)
                    {
                        if (channel != HeartbeatChannel)
                        {
                            HeartbeatChannel.RestartTimeoutTimer();
                        }
                        if (channel?.Logger != null) LogReceivedMessage(channel.Logger, payload, null);

                        var message = JsonSerializer.Deserialize(payload, SharpcasteSerializationContext.Default.MessageWithId);
                        if (message != null && MessageTypes.TryGetValue(message.Type, out Type type))
                        {
                            try
                            {
                                channel?.OnMessageReceived(payload, message.Type);
                                if (message.HasRequestId)
                                {
                                    WaitingTasks.TryRemove(message.RequestId, out SharpCasterTaskCompletionSource? tcs);
                                    tcs?.SetResult(payload);
                                    if (tcs == null)
                                        if (_logger != null) LogNoTaskCompletionSource(_logger, message.RequestId, WaitingTasks.Count, message.Type, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (_logger != null) LogExceptionProcessingResponse(_logger, ex.Message, ex);
                                if (message.HasRequestId)
                                {
                                    WaitingTasks.TryRemove(message.RequestId, out SharpCasterTaskCompletionSource? tcs);
                                    tcs?.SetException(ex);
                                }
                            }
                        }
                        else
                        {
                            if (_logger != null) LogMessageConversionError(_logger, message.Type, null);
                            Debugger.Break();
                        }
                    }
                    else
                    {
                        if (_logger != null) LogChannelParseError(_logger, castMessage.Namespace, payload, null);
                    }
                }
            }
            catch (Exception exception)
            {
                if (_logger != null) LogReceiveLoopError(_logger, exception.Message, exception);
                ReceiveTcs.SetResult(true);
            }
        }, cancellationToken).ConfigureAwait(false);

        public async Task SendAsync(ILogger? logger, string ns, string messagePayload, string destinationId)
        {
            var castMessage = CreateCastMessage(ns, destinationId);
            castMessage.PayloadUtf8 = messagePayload;
            await SendAsync(logger, castMessage).ConfigureAwait(false);
        }

        private async Task SendAsync(ILogger? logger, CastMessage castMessage)
        {
            await SendSemaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                if (logger != null) LogSentMessage(logger, castMessage.Namespace, castMessage.DestinationId, castMessage.PayloadUtf8, null);
#if NETSTANDARD2_0
                byte[] message = castMessage.ToProto();
#else
                ReadOnlyMemory<byte> message = castMessage.ToProto();
#endif
#if NETSTANDARD2_0
                await _stream!.WriteAsync(message, 0, message.Length);
#else
                await _stream!.WriteAsync(message).ConfigureAwait(false);
#endif
            }
            finally
            {
                SendSemaphoreSlim.Release();
            }
        }

        private CastMessage CreateCastMessage(string ns, string destinationId) => new CastMessage()
        {
            Namespace = ns,
            SourceId = SenderId.ToString(),
            DestinationId = destinationId
        };

        public async Task<string> SendAsync(ILogger? logger, string ns, int messageRequestId, string messagePayload, string destinationId)
        {
            var taskCompletionSource = new SharpCasterTaskCompletionSource();
            WaitingTasks[messageRequestId] = taskCompletionSource;
            await SendAsync(logger, ns, messagePayload, destinationId).ConfigureAwait(false);
            return await taskCompletionSource.Task.TimeoutAfter(RECEIVE_TIMEOUT).ConfigureAwait(false);
        }

        public async Task<string> WaitResponseAsync(int messageRequestId)
        {
            var taskCompletionSource = new SharpCasterTaskCompletionSource();
            WaitingTasks[messageRequestId] = taskCompletionSource;
            return await taskCompletionSource.Task.TimeoutAfter(RECEIVE_TIMEOUT).ConfigureAwait(false);
        }

        public async Task DisconnectAsync()
        {
            foreach (var task in WaitingTasks)
            {
                _logger?.LogDebug("Cancelling task for RequestId: {RequestId}", task.Key);
                task.Value?.SetException(new TaskCanceledException("Client disconnected before receiving response."));
            }
            WaitingTasks.Clear();

            if (HeartbeatChannel != null)
            {
                HeartbeatChannel.StopTimeoutTimer();
                HeartbeatChannel.StatusChanged -= HeartBeatTimedOut;
                HeartbeatChannel.Dispose();
            }


            // Recreate HeartbeatChannel since it's been disposed
            RecreateHeartbeatChannel();

            _cancellationTokenSource.Cancel(true);
            await Task.Delay(100).ConfigureAwait(false);
            await Dispose().ConfigureAwait(false);
        }

        public async Task Dispose() => await Dispose(true).ConfigureAwait(false);

        private async Task Dispose(bool waitReceiveTask)
        {
            if (_client != null)
            {
                WaitingTasks.Clear();
                Dispose(_stream!, () => _stream = null);
                Dispose(_client, () => _client = null);
                if (waitReceiveTask && ReceiveTcs != null)
                {
                    await ReceiveTcs.Task.ConfigureAwait(false);
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
                    if (_logger != null) LogDisposeError(_logger, ex.Message, ex);
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
        protected virtual void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Recreates the HeartbeatChannel after disposal
        /// </summary>
        private void RecreateHeartbeatChannel()
        {
            // Remove the disposed HeartbeatChannel from the channels collection
            var channelsList = Channels.ToList();
            var disposedHeartbeat = channelsList.OfType<HeartbeatChannel>().FirstOrDefault();
            if (disposedHeartbeat != null)
            {
                channelsList.Remove(disposedHeartbeat);

                // Create a new HeartbeatChannel instance using the service provider
                var newHeartbeat = _serviceProvider.GetService<HeartbeatChannel>() ??
                                   new HeartbeatChannel(_serviceProvider.GetService<ILogger<HeartbeatChannel>>());
                newHeartbeat.Client = this;
                channelsList.Add(newHeartbeat);

                // Update the channels collection
                Channels = channelsList;
            }
        }

        /// <summary>
        /// Gets a channel
        /// </summary>
        /// <typeparam name="TChannel">channel type</typeparam>
        /// <returns>a channel</returns>
        public TChannel GetChannel<TChannel>() where TChannel : IChromecastChannel
        {
            var channel = Channels.OfType<TChannel>().First();
            if (channel == null)
            {
                throw new ArgumentNullException($"Channel of type {typeof(TChannel).Name} not found.");
            }
            return channel;
        }

        public async Task<ChromecastStatus?> LaunchApplicationAsync(string applicationId, bool joinExistingApplicationSession = true)
        {
            if (joinExistingApplicationSession)
            {
                var status = ChromecastStatus;
                var runningApplication = status?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
                if (runningApplication != null)
                {
                    await ConnectionChannel.ConnectAsync(runningApplication.TransportId).ConfigureAwait(false);
                    //Check if the application is using the media namespace
                    //If so go and get the media status
                    if (runningApplication.Namespaces.Where(ns => ns.Name == "urn:x-cast:com.google.cast.media") != null)
                    {
                        await MediaChannel.GetMediaStatusAsync().ConfigureAwait(false);
                    }
                    return await ReceiverChannel.GetChromecastStatusAsync().ConfigureAwait(false);
                }
            }
            var newApplication = await ReceiverChannel.LaunchApplicationAsync(applicationId).ConfigureAwait(false);
            await ConnectionChannel.ConnectAsync(newApplication.Application.TransportId).ConfigureAwait(false);
            return await ReceiverChannel.GetChromecastStatusAsync().ConfigureAwait(false);
        }


        public ChromecastStatus ChromecastStatus => ReceiverChannel.ReceiverStatus;

        public MediaStatus? MediaStatus => MediaChannel.MediaStatus;
    }

    /// <summary>
    /// A logger wrapper that uses the main ChromecastClient logger but prefixes logs with the channel name
    /// </summary>
    /// <typeparam name="T">The channel type</typeparam>
    internal class ChannelLogger<T> : ILogger<T>
    {
        private readonly ILogger _baseLogger;
        private readonly string _categoryName;

        public ChannelLogger(ILogger baseLogger)
        {
            _baseLogger = baseLogger;
            _categoryName = typeof(T).Name;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _baseLogger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _baseLogger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var originalMessage = formatter(state, exception);
            var prefixedMessage = $"[{_categoryName}] {originalMessage}";

            _baseLogger.Log(logLevel, eventId, prefixedMessage, exception, (msg, ex) => msg);
        }
    }
}