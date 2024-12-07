using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Load message
    /// </summary>
    public class LoadMessage : MessageWithSession
    {
        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [JsonPropertyName("media")]
        public Models.Media.Media Media { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the media must be played directly or not
        /// </summary>
        [JsonPropertyName("autoplay")]
        public bool AutoPlay { get; set; }
    }
}
