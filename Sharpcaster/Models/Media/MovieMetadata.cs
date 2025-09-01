using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Movie metadata
    /// </summary>
    public class MovieMetadata : MediaMetadata
    {
        /// <summary>
        /// Initializes a new instance of MovieMetadata class
        /// </summary>
        public MovieMetadata()
        {
            MetadataType = MetadataType.Movie;
        }

        /// <summary>
        /// Gets or sets the studio
        /// </summary>
        [JsonPropertyName("studio")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Studio { get; set; }
    }
}
