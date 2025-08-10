using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media metadata
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.MediaMetadata">Google Cast MediaMetadata Documentation</see>
    [JsonDerivedType(typeof(MovieMetadata))]
    [JsonDerivedType(typeof(TVShowMetadata))]
    [JsonDerivedType(typeof(MusicTrackMetadata))]
    [JsonDerivedType(typeof(PhotoMetadata))]
    [JsonDerivedType(typeof(AudiobookChapterMetadata))]
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle
        /// </summary>
        [JsonPropertyName("subtitle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SubTitle { get; set; }

        /// <summary>
        /// Gets or sets the images
        /// </summary>
        [JsonPropertyName("images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Image[]? Images { get; set; }
    }
}
