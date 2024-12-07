using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Get or sets the media
    /// </summary>
    [DataContract]
    public class Media
    {
        /// <summary>
        /// Gets or sets the content identifier
        /// </summary>
        [JsonPropertyName("contentId")]
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
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonPropertyName("metadata")]
        public MediaMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the duration of the media
        /// </summary>
        [JsonPropertyName("duration")]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the custom data
        /// </summary>
        [JsonPropertyName("customData")]
        public IDictionary<string, string> CustomData { get; set; }
    }
}
