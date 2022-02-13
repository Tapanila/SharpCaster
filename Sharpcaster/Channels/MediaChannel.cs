using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Media;
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
        public MediaChannel() : base("media")
        {
        }


        private async Task<MediaStatus> SendAsync(IMessageWithId message, ChromecastApplication application)
        {
            try
            {
                return (await SendAsync<MediaStatusMessage>(message, application.TransportId)).Status?.FirstOrDefault();
            }
            catch (Exception)
            {
                Status = null;
                throw;
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
    }
}
