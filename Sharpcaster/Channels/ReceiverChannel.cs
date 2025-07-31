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
    public class ReceiverChannel : ChromecastChannel
    {
        public ChromecastStatus ReceiverStatus { get => receiverStatus; }
        private ChromecastStatus receiverStatus = new();

        private static readonly Action<ILogger, double, Exception?> LogInvalidVolumeLevel =
            LoggerMessage.Define<double>(LogLevel.Error, new EventId(3001, "InvalidVolumeLevel"), "level must be between 0.0 and 1.0 - is {Level}");

        public ReceiverChannel(ILogger<ReceiverChannel>? logger = null) : base("receiver", logger)
        {
        }
        public event EventHandler<LaunchStatusMessage>? LaunchStatusChanged;
        public event EventHandler<ChromecastStatus>? ReceiverStatusChanged;

        public async Task<ChromecastStatus?> GetChromecastStatusAsync()
        {
            var getStatusMessage = new GetStatusMessage();
            var response = await SendAsync(getStatusMessage.RequestId, JsonSerializer.Serialize(getStatusMessage, SharpcasteSerializationContext.Default.GetStatusMessage)).ConfigureAwait(false);
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status?.Status;
        }

        public async Task<ChromecastStatus?> LaunchApplicationAsync(string applicationId)
        {
            var launchMessage = new LaunchMessage() { ApplicationId = applicationId };
            var response = await SendAsync(launchMessage.RequestId, JsonSerializer.Serialize(launchMessage, SharpcasteSerializationContext.Default.LaunchMessage)).ConfigureAwait(false);
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status?.Status;
        }

        public async Task<ChromecastStatus?> SetMute(bool muted)
        {
            var setVolumeMessage = new SetVolumeMessage() { Volume = new Models.Volume() { Muted = muted } };
            var response = await SendAsync(setVolumeMessage.RequestId, JsonSerializer.Serialize(setVolumeMessage, SharpcasteSerializationContext.Default.SetVolumeMessage)).ConfigureAwait(false);
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status?.Status;
        }

        public async Task<ChromecastStatus?> SetVolume(double level)
        {
            if (level < 0 || level > 1.0)
            {
                if (Logger != null) LogInvalidVolumeLevel(Logger, level, null);
                throw new ArgumentException("level must be between 0.0 and 1.0", nameof(level));
            }
            var setVolumeMessage = new SetVolumeMessage() { Volume = new Models.Volume() { Level = level } };
            var response = await SendAsync(setVolumeMessage.RequestId, JsonSerializer.Serialize(setVolumeMessage, SharpcasteSerializationContext.Default.SetVolumeMessage)).ConfigureAwait(false);
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);

            if (status?.Status.Volume?.Level != level)
            {
                response = await SendAsync(setVolumeMessage.RequestId, JsonSerializer.Serialize(setVolumeMessage, SharpcasteSerializationContext.Default.SetVolumeMessage)).ConfigureAwait(false);
                status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            }

            return status?.Status;
        }

        public async Task<ChromecastStatus?> StopApplication()
        {
            var stopMessage = new StopMessage() { SessionId = ReceiverStatus.Application.SessionId };
            var response = await SendAsync(stopMessage.RequestId, JsonSerializer.Serialize(stopMessage, SharpcasteSerializationContext.Default.StopMessage)).ConfigureAwait(false);
            var status = JsonSerializer.Deserialize(response, SharpcasteSerializationContext.Default.ReceiverStatusMessage);
            return status?.Status;
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
                    if (receiverStatusMessage?.Status != null)
                    {
                        receiverStatus = receiverStatusMessage.Status;
                        ReceiverStatusChanged?.Invoke(this, ReceiverStatus);
                    }
                    break;

            }
            return base.OnMessageReceivedAsync(messagePayload, type);
        }
    }
}