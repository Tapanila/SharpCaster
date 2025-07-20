using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Metadata for container-type media objects
    /// </summary>
    public class ContainerMetadata
    {
        /// <summary>
        /// Container duration in seconds
        /// For example an audiobook playback time
        /// </summary>
        [JsonPropertyName("containerDuration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? ContainerDuration { get; set; }

        /// <summary>
        /// Container images
        /// For example a live TV channel logo, audiobook cover, album cover art, etc.
        /// </summary>
        [JsonPropertyName("containerImages")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Image[]? ContainerImages { get; set; }

        /// <summary>
        /// The type of container object
        /// </summary>
        [JsonPropertyName("containerType")]
        public ContainerType ContainerType { get; set; }

        /// <summary>
        /// Array of media metadata objects to describe the media content sections
        /// Used to delineate live TV streams into programs and audiobooks into chapters
        /// </summary>
        [JsonPropertyName("sections")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MediaMetadata[]? Sections { get; set; }

        /// <summary>
        /// The title of the container
        /// For example an audiobook title, a TV channel name, etc.
        /// </summary>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }
    }
}