using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Base class for status channels
    /// </summary>
    /// <typeparam name="TStatusMessage">status message type</typeparam>
    /// <typeparam name="TStatus">status type</typeparam>
    public abstract class StatusChannel<TStatusMessage, TStatus> : ChromecastChannel, IStatusChannel<TStatus> where TStatusMessage : IStatusMessage<TStatus>
    {
        /// <summary>
        /// Raised when the status has changed
        /// </summary>
        public event EventHandler<TStatus> StatusChanged;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="ns">namespace</param>
        protected StatusChannel(string ns, ILogger logger) : base(ns, logger)
        {
        }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public TStatus Status { get; protected set; }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public override Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            switch (type)
            {
                case TStatusMessage statusMessage:
                    Status = statusMessage.Status;
                    OnStatusChanged(statusMessage.Status);
                    break;
            }

            return base.OnMessageReceivedAsync(messagePayload, type);
        }

        /// <summary>
        /// Raises the StatusChanged event
        /// </summary>
        protected virtual void OnStatusChanged(TStatus status)
        {
            StatusChanged?.Invoke(this, status);
        }

        public void ClearStatus()
        {
            Status = default;
        }
    }
}
