using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Media;
using Sharpcaster.Messages.Queue;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Collections.Generic;
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
        /// Initializes a new instance of MediaChannel class
        /// </summary>
        public MediaChannel(ILogger<MediaChannel> logger = null) : base("media", logger)
        {
        }


        private async Task<MediaStatus> SendAsync(IMessageWithId message, ChromecastApplication application)
        {
            try
            {
                var response = await SendAsync<IMessageWithId>(message, application.TransportId);
                if (response is LoadFailedMessage)
                {
                    throw new Exception("Load failed");
                }
                if (response is LoadCancelledMessage)
                {
                    throw new Exception("Load cancelled");
                }
                return (response as MediaStatusMessage).Status?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error sending message: {ex.Message}");
                Status = null;
                throw ex;
            }
        }

        private async Task<MediaStatus> SendAsync(MediaSessionMessage message)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            message.MediaSessionId = Status?.First().MediaSessionId ?? throw new ArgumentNullException("MediaSessionId");
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
            return await SendAsync(new LoadMessage() { SessionId = status.Applications[0].SessionId, Media = media, AutoPlay = autoPlay }, status.Applications[0]);
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

        public async Task<MediaStatus> QueueLoadAsync(QueueItem[] items)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<MediaStatusMessage>(new QueueLoadMessage() { SessionId = chromecastStatus.Applications[0].SessionId, Items = items }, chromecastStatus.Applications[0].TransportId)).Status?.FirstOrDefault();
        }

        public async Task<MediaStatus> QueueNextAsync(long mediaSessionId)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<MediaStatusMessage>(new QueueNextMessage() { MediaSessionId = mediaSessionId }, chromecastStatus.Applications[0].TransportId)).Status?.FirstOrDefault();
        }

        public async Task<MediaStatus> QueuePrevAsync(long mediaSessionId)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<MediaStatusMessage>(new QueuePrevMessage() { MediaSessionId = mediaSessionId }, chromecastStatus.Applications[0].TransportId)).Status?.FirstOrDefault();
        }


        public async Task<QueueItem[]> QueueGetItemsAsync(long mediaSessionId, int[] ids = null)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<QueueItemsMessage>(new QueueGetItemsMessage() { MediaSessionId = mediaSessionId, Ids = ids }, chromecastStatus.Applications[0].TransportId)).Items;
        }

        public async Task<int[]> QueueGetItemIdsAsync(long mediaSessionId)
        {
            var chromecastStatus = Client.GetChromecastStatus();
            return (await SendAsync<QueueItemIdsMessage>(new QueueGetItemIdsMessage() { MediaSessionId = mediaSessionId }, chromecastStatus.Applications[0].TransportId)).Ids;
        }

    }
}
