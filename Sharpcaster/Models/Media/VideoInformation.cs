using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents video-specific information
    /// </summary>
    public class VideoInformation
    {
        /// <summary>
        /// Gets or sets the video width in pixels
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the video height in pixels
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the high dynamic range (HDR) type
        /// </summary>
        [JsonPropertyName("hdrType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HdrType? HdrType { get; set; }
    }

    /// <summary>
    /// High Dynamic Range types
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<HdrType>))]
    public enum HdrType
    {
        /// <summary>
        /// Standard Dynamic Range
        /// </summary>
        SDR,

        /// <summary>
        /// High Dynamic Range 10
        /// </summary>
        HDR,

        /// <summary>
        /// Dolby Vision
        /// </summary>
        DV
    }
}