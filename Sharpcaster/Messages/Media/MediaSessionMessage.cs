using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Media session message
    /// </summary>
    public abstract class MediaSessionMessage : MessageWithId
    {
        /// <summary>
        /// Gets or sets the media session identifier
        /// </summary>
        [JsonPropertyName("mediaSessionId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? MediaSessionId { get; set; }
    }
}
