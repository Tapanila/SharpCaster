using Sharpcaster.Converters;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    /// <summary>
    /// A request to update properties of the existing items in the media queue.
    /// </summary>
    public class QueueUpdateMessage : MediaSessionMessage
    {
        [JsonPropertyName("currentItemId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CurrentItemId { get; set; }

        [JsonPropertyName("currentTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CurrentTime { get; set; }
        /// <summary>
        /// List of queue items to be updated. No reordering will happen, the items will retain the existing order and will be fully replaced with the ones provided,
        /// including the media information. The items not provided in this list will remain unchanged.
        /// The tracks information can not change once the item is loaded (if the item is the currentItem). If any of the items does not exist it will be ignored.
        /// </summary>
        [JsonPropertyName("items")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public QueueItem[] Items { get; set; }
        [JsonPropertyName("jump")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Jump { get; set; }
        /// <summary>
        /// Behavior of the queue when all items have been played.
        /// </summary>
        [JsonPropertyName("repeatMode")]
        [JsonConverter(typeof(RepeatModeEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RepeatModeType? RepeatMode { get; set; }
        [JsonPropertyName("sequenceNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SequenceNumber { get; set; }
        [JsonPropertyName("shuffle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Shuffle { get; set; }

    }
}
