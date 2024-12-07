using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Messages.Media;
using Sharpcaster.Messages.Queue;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Media channel
    /// </summary>
    public class MediaChannel : StatusChannel<MediaStatusMessage, IEnumerable<MediaStatus>>, IMediaChannel
    {
        /// <summary>
        /// Raised when error is received
        /// </summary>
        public event EventHandler<ErrorMessage> ErrorHappened;
        public event EventHandler<LoadCancelledMessage> LoadCancelled;
        public event EventHandler<LoadFailedMessage> LoadFailed;    
        public event EventHandler<InvalidRequestMessage> InvalidRequest;
        public MediaStatus MediaStatus { get => Status.FirstOrDefault(); }
        /// <summary>
        /// Initializes a new instance of MediaChannel class
        /// </summary>
        public MediaChannel(ILogger<MediaChannel> logger = null) : base("media", logger)
        {
        }

        private async Task<MediaStatus> SendAsync(IMessageWithId message, ChromecastApplication application, bool DoNotReturnOnLoading = true)
        {
            try
            {
                var response = await SendAsync<IMessageWithId>(message, application.TransportId);
                if (DoNotReturnOnLoading && (response as MediaStatusMessage).Status.FirstOrDefault().ExtendedStatus?.PlayerState == PlayerStateType.Loading)
                    response = await Client.WaitResponseAsync<IMessageWithId>(message);
                return (response as MediaStatusMessage).Status?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger?.LogError("Error sending message: {exceptionMessage}", ex.Message);
                Status = null;
                throw;
            }
        }

        private async Task<MediaStatus> SendAsync(MediaSessionMessage message)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            message.MediaSessionId = Status?.First().MediaSessionId ?? throw new ArgumentNullException(nameof(message), "MediaSessionID");
            return await SendAsync(message, chromecastStatus.Applications[0]);
        }

        /// <summary>
        /// Loads a media
        /// </summary>
        /// <param name="media">media to load</param>
        /// <param name="autoPlay">true to play the media directly, false otherwise</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus> LoadAsync(Media media, bool autoPlay = true)
        {
            var status = Client.GetChromecastStatus();
            return await SendAsync(new LoadMessage() { SessionId = status.Application.SessionId, Media = media, AutoPlay = autoPlay }, status.Application);
        }

        public override Task OnMessageReceivedAsync(IMessage message)
        {
            switch (message)
            {
                case LoadFailedMessage loadFailedMessage:
                    LoadFailed?.Invoke(this, loadFailedMessage);
                    return Task.CompletedTask;
                case LoadCancelledMessage loadCancelledMessage:
                    LoadCancelled?.Invoke(this, loadCancelledMessage);
                    return Task.CompletedTask;
                case InvalidRequestMessage invalidRequestMessage:
                    InvalidRequest?.Invoke(this, invalidRequestMessage);
                    return Task.CompletedTask;
                case ErrorMessage errorMessage:
                    ErrorHappened?.Invoke(this, errorMessage);
                    return Task.CompletedTask;
            }
            return base.OnMessageReceivedAsync(message);
        }

        /// <summary>
        /// Plays the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus> PlayAsync()
        {
            return await SendAsync(new PlayMessage());
        }

        /// <summary>
        /// Pauses the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus> PauseAsync()
        {
            return await SendAsync(new PauseMessage());
        }

        /// <summary>
        /// Stops the media
        /// </summary>
        /// <returns>media status</returns>
        public async Task<MediaStatus> StopAsync()
        {
            return await SendAsync(new StopMessage());
        }

        /// <summary>
        /// Seeks to the specified time
        /// </summary>
        /// <param name="seconds">time in seconds</param>
        /// <returns>media status</returns>
        public async Task<MediaStatus> SeekAsync(double seconds)
        {
            return await SendAsync(new SeekMessage() { CurrentTime = seconds });
        }

        public async Task<MediaStatus> QueueLoadAsync(QueueItem[] items, long? currentTime = null, RepeatModeType repeatMode = RepeatModeType.OFF, long? startIndex = null)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            var response = await SendAsync<MessageWithId>(new QueueLoadMessage() { SessionId = chromecastStatus.Application.SessionId, Items = items, CurrentTime = currentTime, RepeatMode = repeatMode, StartIndex = startIndex }, chromecastStatus.Application.TransportId);

            switch (response)
            {
                case MediaStatusMessage mediaStatusMessage:
                    return mediaStatusMessage.Status?.FirstOrDefault();
            }
            return null;
        }

        public async Task<MediaStatus> QueueNextAsync()
        {
            return await SendAsync(new QueueNextMessage());
        }

        public async Task<MediaStatus> QueuePrevAsync()
        {
            return await SendAsync(new QueuePrevMessage());
        }

        public async Task<QueueItem[]> QueueGetItemsAsync(int[] ids = null)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<QueueItemsMessage>(new QueueGetItemsMessage() { MediaSessionId = MediaStatus.MediaSessionId, Ids = ids }, chromecastStatus.Application.TransportId)).Items;
        }

        public async Task<int[]> QueueGetItemIdsAsync()
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<QueueItemIdsMessage>(new QueueGetItemIdsMessage() { MediaSessionId = MediaStatus.MediaSessionId }, chromecastStatus.Application.TransportId)).Ids;
        }

        public async Task<MediaStatus> GetMediaStatusAsync()
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return await SendAsync(new Messages.Receiver.GetStatusMessage(), chromecastStatus.Application);
        }
    }
}
