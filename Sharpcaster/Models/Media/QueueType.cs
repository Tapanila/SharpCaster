using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Types of media queues
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<QueueType>))]
    public enum QueueType
    {
        /// <summary>
        /// Album queue type
        /// </summary>
        ALBUM,

        /// <summary>
        /// Playlist queue type
        /// </summary>
        PLAYLIST,

        /// <summary>
        /// Audiobook queue type
        /// </summary>
        AUDIOBOOK,

        /// <summary>
        /// Radio station queue type
        /// </summary>
        RADIO_STATION,

        /// <summary>
        /// Podcast series queue type
        /// </summary>
        PODCAST_SERIES,

        /// <summary>
        /// TV series queue type
        /// </summary>
        TV_SERIES,

        /// <summary>
        /// Video playlist queue type
        /// </summary>
        VIDEO_PLAYLIST,

        /// <summary>
        /// Live TV queue type
        /// </summary>
        LIVE_TV,

        /// <summary>
        /// Movie queue type
        /// </summary>
        MOVIE
    }
}