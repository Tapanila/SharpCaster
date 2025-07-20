using System.Text.Json.Serialization;
using Sharpcaster.Messages;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// A request to modify the text tracks style or change the tracks status
    /// </summary>
    public class EditTracksInfoRequest : MessageWithId
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
    }
}