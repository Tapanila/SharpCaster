using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Models.MultiZone;
using System;
using System.Text.Json;
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
        public override Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            switch (type)
            {
                case "MULTIZONE_STATUS":
                    var multizoneStatusMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.MultizoneStatusMessage);
                    Status = multizoneStatusMessage.Status;
                    StatusChanged?.Invoke(this, multizoneStatusMessage.Status);
                    break;
                case "DEVICE_UPDATED":
                    var deviceUpdatedMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.DeviceUpdatedMessage);
                    DeviceUpdated?.Invoke(this, deviceUpdatedMessage.Device);
                    break;
                default:
                    break;
            }

            return base.OnMessageReceivedAsync(messagePayload, type);
        }

        /// <summary>
        /// Raises the StatusChanged event
        /// </summary>
        protected virtual void OnStatusChanged()
        {
        }
    }
}
