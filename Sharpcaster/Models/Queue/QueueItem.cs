using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Queue
{
    public class QueueItem
    {
        /// <summary>
        /// Gets or sets the item identifier
        /// </summary>
        [JsonPropertyName("itemId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? ItemId { get; set; }
        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [JsonPropertyName("media")]
        public Media.Media Media { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether autoplay is enabled or not
        /// </summary>
        [JsonPropertyName("autoPlay")]
        public bool IsAutoPlay { get; set; } = true;

        [JsonPropertyName("orderId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? OrderId { get; set; }

        [JsonPropertyName("startTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? StartTime { get; set; }

        [JsonPropertyName("preloadTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? PreloadTime { get; set; }

        [JsonPropertyName("activeTrackIds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long[] ActiveTrackIds { get; set; }
    }
}
