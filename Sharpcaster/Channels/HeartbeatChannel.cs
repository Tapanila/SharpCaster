using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Heartbeat;
using Sharpcaster.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Heartbeat channel. Responds to ping messages with pong message
    /// </summary>
    public class HeartbeatChannel : ChromecastChannel, IHeartbeatChannel
    {
        //private ILogger _logger = null;
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of HeartbeatChannel class
        /// </summary>
        public HeartbeatChannel(ILogger<HeartbeatChannel> logger = null) : base("tp.heartbeat")
        {
            _logger = logger; 
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
        public override async Task OnMessageReceivedAsync(IMessage message)
        {
            _logger?.LogDebug("Received ping message on Heartbeat channel");
            await SendAsync(new PongMessage());
            _logger?.LogDebug("Sent pong message on Heartbeat channel");
            _timer.Stop();
            _timer.Start();
        }

        public void StartTimeoutTimer()
        {
            _timer.Start();
            _logger?.LogDebug("Started heartbeat timeout timer");
        }

        public void StopTimeoutTimer()
        {
            _timer.Stop();
            _logger?.LogDebug("Stopped heartbeat timeout timer");
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _logger?.LogDebug("Heartbeat timeout");
            StatusChanged?.Invoke(this, e);
        }
    }
}
