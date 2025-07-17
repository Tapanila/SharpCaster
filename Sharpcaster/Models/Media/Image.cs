using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Image
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Gets or sets the image url
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the image height
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the image width
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }
    }
}
