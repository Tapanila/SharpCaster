using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces {
    /// <summary>
    /// Interface for the media channel
    /// </summary>
    public interface IMediaChannel : IStatusChannel<IEnumerable<MediaStatus>>, IChromecastChannel {
        /// <summary>
        /// Loads a media
        /// </summary>
        /// <param name="media">media to load</param>
        /// <param name="autoPlay">true to play the media directly, false otherwise</param>
        /// <returns>media status</returns>
        Task<MediaStatus> LoadAsync(Media media, bool autoPlay = true);

        /// <summary>
        /// Plays the media
        /// </summary>
        /// <returns>media status</returns>
        Task<MediaStatus> PlayAsync();

        /// <summary>
        /// Pauses the media
        /// </summary>
        /// <returns>media status</returns>
        Task<MediaStatus> PauseAsync();

        /// <summary>
        /// Stops the media
        /// </summary>
        /// <returns>media status</returns>
        Task<MediaStatus> StopAsync();

        /// <summary>
        /// Seeks to the specified time
        /// </summary>
        /// <param name="seconds">time in seconds</param>
        /// <returns>media status</returns>
        Task<MediaStatus> SeekAsync(double seconds);
        Task<MediaStatus> QueueLoadAsync(QueueItem[] items);
        Task<MediaStatus> QueueNextAsync(long mediaSessionId);
        Task<MediaStatus> QueuePrevAsync(long mediaSessionId);
        Task<QueueItem[]> QueueGetItemsAsync(long mediaSessionId, int[] ids = null);
        Task<int[]> QueueGetItemIdsAsync(long mediaSessionId);
    }
}
