using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Receiver;
using Sharpcaster.Models.ChromecastStatus;
using System;
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

        public async Task<ChromecastStatus> GetChromecastStatusAsync()
        {
            return (await SendAsync<ReceiverStatusMessage>(new GetStatusMessage())).Status;
        }

        public async Task<ChromecastStatus> LaunchApplicationAsync(string applicationId)
        {
            return (await SendAsync<ReceiverStatusMessage>(new LaunchMessage() { ApplicationId = applicationId })).Status;
        }

        public async Task<ChromecastStatus> SetMute(bool muted)
        {
            return (await SendAsync<ReceiverStatusMessage>(new SetVolumeMessage() { Volume = new Models.Volume() { Muted = muted } })).Status;
        }

        public async Task<ChromecastStatus> SetVolume(double level)
        {
            if (level < 0 || level > 1.0)
            {
                _logger?.LogError($"level must be between 0.0 and 1.0 - is {level}");
                throw new ArgumentException("level must be between 0.0 and 1.0", nameof(level));
            }
            return (await SendAsync<ReceiverStatusMessage>(new SetVolumeMessage() { Volume = new Models.Volume() { Level = level } })).Status;
        }

        public async Task<ChromecastStatus> StopApplication(string sessionId)
        {
            return (await SendAsync<ReceiverStatusMessage>(new StopMessage() { SessionId = sessionId })).Status;
        }
    }
}