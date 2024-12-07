using System.Text.Json.Serialization;
using Sharpcaster.Converters;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Models.Queue
{
    /// <summary>
    /// Represents the metadata for a media queue.
    /// </summary>
    public class QueueData
    {
        /// <summary>
        /// The description of the queue.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// An optional Queue entity ID, providing a Google Assistant deep link.
        /// </summary>
        [JsonPropertyName("entity")]
        public string Entity { get; set; }

        /// <summary>
        /// The ID of the queue.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// An Array of queue items, sorted in playback order.
        /// </summary>
        [JsonPropertyName("items")]
        public QueueItem[] Items { get; set; }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A queue type, such as album, playlist, radio station, or tv series.
        /// </summary>
        [JsonPropertyName("queueType")]
        public string QueueType { get; set; }

        /// <summary>
        /// The continuous playback behavior of the queue.
        /// </summary>
        [JsonPropertyName("repeatMode")]
        [JsonConverter(typeof(RepeatModeEnumConverter))]
        public RepeatModeType RepeatMode { get; set; }

        /// <summary>
        /// True indicates that the queue is shuffled.
        /// </summary>
        [JsonPropertyName("shuffle")]
        public bool IsShuffle { get; set; }

        /// <summary>
        /// The index of the item in the queue that should be used to start playback first.
        /// </summary>
        [JsonPropertyName("startIndex")]
        public long? StartIndex { get; set; }

        /// <summary>
        /// When to start playback of the first item, expressed as the number of seconds since the beginning of the media.
        /// </summary>
        [JsonPropertyName("startTime")]
        public long? StartTime { get; set; }
    }
}