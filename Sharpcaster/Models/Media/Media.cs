using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Get or sets the media
    /// </summary>
    public class Media
    {
        /// <summary>
        /// Gets or sets the content identifier
        /// </summary>
        [JsonPropertyName("contentId")]
        public string ContentId { get; set; }
        /// <summary>
        /// Gets or sets the content identifier
        /// </summary>
        [JsonPropertyName("contentUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the stream type
        /// </summary>
        [JsonPropertyName("streamType")]
        public StreamType StreamType { get; set; } = StreamType.Buffered;

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        [JsonPropertyName("contentType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MediaMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the duration of the media
        /// </summary>
        [JsonPropertyName("duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the custom data
        /// </summary>
        [JsonPropertyName("customData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, string> CustomData { get; set; }
    }
}
