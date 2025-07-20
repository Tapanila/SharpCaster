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
        /// Gets or sets the edit tracks info request
        /// </summary>
        [JsonPropertyName("editTracksInfoRequest")]
        public EditTracksInfoRequest EditTracksInfoRequest { get; set; }
    }
}