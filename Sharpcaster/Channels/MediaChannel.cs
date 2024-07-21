﻿using Microsoft.Extensions.Logging;
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
        public MediaStatus MediaStatus { get => Status.FirstOrDefault(); }
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
                _logger?.LogError("Error sending message: {Message}", ex.Message);
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
            return (await SendAsync<MediaStatusMessage>(new QueueLoadMessage() { SessionId = chromecastStatus.Application.SessionId, Items = items, CurrentTime = currentTime, RepeatMode = repeatMode, StartIndex = startIndex }, chromecastStatus.Application.TransportId)).Status?.FirstOrDefault();
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
