using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Receiver;
using Sharpcaster.Models.ChromecastStatus;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// ReceiverChannel, Receives ChromecastStatus, volume, starting and stopping application
    /// </summary>
    public class ReceiverChannel : StatusChannel<ReceiverStatusMessage, ChromecastStatus>, IReceiverChannel
    {
        public ReceiverChannel(ILogger<ReceiverChannel> logger = null) : base("receiver", logger)
        {
        }

        public event EventHandler<LaunchStatusMessage> LaunchStatusChanged;

        public async Task<ChromecastStatus> GetChromecastStatusAsync()
        {
            var getStatusMessage = new GetStatusMessage();
            var response = await SendAsync(getStatusMessage.RequestId, JsonSerializer.Serialize(getStatusMessage, SharpcasteSerializationContext.Default.GetStatusMessage));
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status.Status;
        }

        public async Task<ChromecastStatus> LaunchApplicationAsync(string applicationId)
        {
            var launchMessage = new LaunchMessage() { ApplicationId = applicationId };
            var response = await SendAsync(launchMessage.RequestId, JsonSerializer.Serialize(launchMessage, SharpcasteSerializationContext.Default.LaunchMessage));
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status.Status;
        }

        public async Task<ChromecastStatus> SetMute(bool muted)
        {
            var setVolumeMessage = new SetVolumeMessage() { Volume = new Models.Volume() { Muted = muted } };
            var response = await SendAsync(setVolumeMessage.RequestId, JsonSerializer.Serialize(setVolumeMessage, SharpcasteSerializationContext.Default.SetVolumeMessage));
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status.Status;
        }

        public async Task<ChromecastStatus> SetVolume(double level)
        {
            if (level < 0 || level > 1.0)
            {
                Logger?.LogError("level must be between 0.0 and 1.0 - is {level}", level);
                throw new ArgumentException("level must be between 0.0 and 1.0", nameof(level));
            }
            var setVolumeMessage = new SetVolumeMessage() { Volume = new Models.Volume() { Level = level } };
            var response = await SendAsync(setVolumeMessage.RequestId, JsonSerializer.Serialize(setVolumeMessage, SharpcasteSerializationContext.Default.SetVolumeMessage));
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status.Status;
        }

        public async Task<ChromecastStatus> StopApplication()
        {
            var stopMessage = new StopMessage() { SessionId = Status.Application.SessionId };
            var response = await SendAsync(stopMessage.RequestId, JsonSerializer.Serialize(stopMessage, SharpcasteSerializationContext.Default.StopMessage));
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status.Status;
        }

        public override Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            switch (type)
            {
                case "LAUNCH_STATUS":
                    var launchStatusMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.LaunchStatusMessage);
                    LaunchStatusChanged?.Invoke(this, launchStatusMessage);
                    break;
                case "RECEIVER_STATUS":
                    var receiverStatusMessage = JsonSerializer.Deserialize(messagePayload, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
                    Status = receiverStatusMessage.Status;
                    OnStatusChanged(receiverStatusMessage.Status);
                    break;

            }
            return base.OnMessageReceivedAsync(messagePayload, type);
        }
    }
}