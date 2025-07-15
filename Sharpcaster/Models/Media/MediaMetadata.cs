using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media metadata
    /// </summary>
    [JsonDerivedType(typeof(MovieMetadata))]
    public class MediaMetadata
    {
        /// <summary>
        /// Gets or sets the metadata type
        /// </summary>
        [JsonPropertyName("metadataType")]
        public MetadataType MetadataType { get; set; } = MetadataType.Default;

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle
        /// </summary>
        [JsonPropertyName("subtitle")]
        public string SubTitle { get; set; }

        /// <summary>
        /// Gets or sets the images
        /// </summary>
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
    }
}
