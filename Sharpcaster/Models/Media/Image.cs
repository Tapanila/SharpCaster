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
        public string Url { get; set; } = null!;

        /// <summary>
        /// Gets or sets the image height
        /// </summary>
        [JsonPropertyName("height")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]   
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the image width
        /// </summary>
        [JsonPropertyName("width")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Width { get; set; }
    }
}
