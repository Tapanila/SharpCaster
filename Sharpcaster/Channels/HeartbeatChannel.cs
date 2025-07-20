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

        /// <summary>
        /// Initializes a new instance of HeartbeatChannel class
        /// </summary>
        public HeartbeatChannel(ILogger<HeartbeatChannel>? logger = null) : base("tp.heartbeat", logger)
        {
            _timer = new Timer(10000); // timeout is 10 seconds.
                                       // Because Chromecast only waits for 8 seconds for response
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = false;
        }

        public event EventHandler StatusChanged;


        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public override async Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            _timer.Stop();
            var pongMessage = new PongMessage();
            await SendAsync(JsonSerializer.Serialize(pongMessage, SharpcasteSerializationContext.Default.PongMessage));
            _timer.Start();
            Logger?.LogDebug("Pong sent - Heartbeat Timer restarted.");
        }

        public void StartTimeoutTimer()
        {
            _timer.Start();
            Logger?.LogTrace("Started heartbeat timeout timer");
        }

        public void StopTimeoutTimer()
        {
            _timer.Stop();
            Logger?.LogTrace("Stopped heartbeat timeout timer");
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logger?.LogInformation("Heartbeat timeout");
            StatusChanged?.Invoke(this, e);
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
