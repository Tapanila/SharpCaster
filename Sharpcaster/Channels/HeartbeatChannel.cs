using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Heartbeat;
using System;
using System.Reactive.Disposables;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Heartbeat channel. Responds to ping messages with pong message
    /// </summary>
    public class HeartbeatChannel : ChromecastChannel, IDisposable
    {
        //private ILogger _logger = null;
        private readonly Timer _timer;
        private bool disposedValue;

        private static readonly Action<ILogger, Exception?> LogPongSent =
            LoggerMessage.Define(LogLevel.Debug, new EventId(1001, nameof(OnMessageReceived)), "Pong sent - Heartbeat Timer restarted.");

        private static readonly Action<ILogger, Exception?> LogHeartbeatTimerStarted =
            LoggerMessage.Define(LogLevel.Trace, new EventId(1002, nameof(StartTimeoutTimer)), "Started heartbeat timeout timer");

        private static readonly Action<ILogger, Exception?> LogHeartbeatTimerStopped =
            LoggerMessage.Define(LogLevel.Trace, new EventId(1003, nameof(StopTimeoutTimer)), "Stopped heartbeat timeout timer");

        private static readonly Action<ILogger, Exception?> LogHeartbeatTimeout =
            LoggerMessage.Define(LogLevel.Information, new EventId(1004, nameof(TimerElapsed)), "Heartbeat timeout");

        /// <summary>
        /// Initializes a new instance of HeartbeatChannel class
        /// </summary>
        public HeartbeatChannel(ILogger? logger = null) : base("tp.heartbeat", logger)
        {
            _timer = new Timer(10000); // timeout is 10 seconds.
                                       // Because Chromecast only waits for 8 seconds for response
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = false;
        }

        public event EventHandler? StatusChanged;


        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="messagePayload">message payload to process</param>
        /// <param name="type">message type</param>
        public override async void OnMessageReceived(string messagePayload, string type)
        {
            _timer.Stop();
            var pongMessage = new PongMessage();
            await SendAsync(JsonSerializer.Serialize(pongMessage, SharpcasteSerializationContext.Default.PongMessage)).ConfigureAwait(false);
            _timer.Start();
            if (Logger != null) LogPongSent(Logger, null);
        }

        public void StartTimeoutTimer()
        {
            _timer.Start();
            if (Logger != null) LogHeartbeatTimerStarted(Logger, null);
        }

        public void StopTimeoutTimer()
        {
            _timer.Stop();
            if (Logger != null) LogHeartbeatTimerStopped(Logger, null);
        }

        public void RestartTimeoutTimer()
        {
            _timer.Stop();
            _timer.Start();
            if (Logger != null) LogHeartbeatTimerStarted(Logger, null);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Logger != null) LogHeartbeatTimeout(Logger, null);
            SafeInvokeEvent(StatusChanged, this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
