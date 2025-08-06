using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Edit tracks info message
    /// </summary>
    public class EditTracksInfoMessage : MediaSessionMessage
    {
        /// <summary>
        /// Array of the Track trackIds that should be active
        /// If not provided, active tracks won't change
        /// If empty array, no track will be active
        /// </summary>
        [JsonPropertyName("activeTrackIds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? ActiveTrackIds { get; set; }

        /// <summary>
        /// The requested text track style
        /// If not provided, existing style will be used
        /// </summary>
        [JsonPropertyName("textTrackStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackStyle? TextTrackStyle { get; set; }
        /// <summary>
        /// Language for the tracks that should be active.
        /// The language field will take precedence over activeTrackIds if both are specified.
        /// </summary>
        [JsonPropertyName("language")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Language { get; set; }
    }
}