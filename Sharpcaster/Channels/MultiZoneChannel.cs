using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Multizone;
using Sharpcaster.Models.MultiZone;
using System;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    public class MultiZoneChannel : ChromecastChannel
    {
        /// <summary>
        /// Raised when the status has changed
        /// </summary>
        public event EventHandler<MultiZoneStatus> StatusChanged;
        
        /// <summary>
        /// Raised when device has been updated
        /// </summary>
        public event EventHandler<Device> DeviceUpdated;

        public MultiZoneStatus Status { get; set; }

        public MultiZoneChannel(ILogger<MultiZoneChannel> logger = null) : base("multizone", logger)
        {

        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public override Task OnMessageReceivedAsync(IMessage message)
        {
            switch (message)
            {
                case MultizoneStatusMessage multizoneStatusMessage:
                    Status = multizoneStatusMessage.Status;
                    StatusChanged?.Invoke(this, multizoneStatusMessage.Status);
                    break;
                case DeviceUpdatedMessage deviceUpdatedMessage:
                    DeviceUpdated?.Invoke(this, deviceUpdatedMessage.Device);
                    break;
                default:
                    break;
            }

            return base.OnMessageReceivedAsync(message);
        }

        /// <summary>
        /// Raises the StatusChanged event
        /// </summary>
        protected virtual void OnStatusChanged()
        {
            
        }
    }
}
