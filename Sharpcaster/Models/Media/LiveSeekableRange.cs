using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents the seekable range for live streams
    /// </summary>
    public class LiveSeekableRange
    {
        /// <summary>
        /// Gets or sets the start time of the seekable range in seconds
        /// </summary>
        [JsonPropertyName("start")]
        public double Start { get; set; }

        /// <summary>
        /// Gets or sets the end time of the seekable range in seconds
        /// </summary>
        [JsonPropertyName("end")]
        public double End { get; set; }

        /// <summary>
        /// Gets or sets whether the range is movable
        /// </summary>
        [JsonPropertyName("isMovingWindow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsMovingWindow { get; set; }

        /// <summary>
        /// Gets or sets whether seeking to the live edge is possible
        /// </summary>
        [JsonPropertyName("isLiveDone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsLiveDone { get; set; }
    }
}