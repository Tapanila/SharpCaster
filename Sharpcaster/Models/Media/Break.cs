using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents a break (such as an ad break) within a video stream
    /// </summary>
    public class Break
    {
        /// <summary>
        /// Unique ID of a break
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// List of break clip IDs included in this break
        /// </summary>
        [JsonPropertyName("breakClipIds")]
        public string[] BreakClipIds { get; set; } = null!;

        /// <summary>
        /// Location of the break inside the main video
        /// -1 represents the end of the main video in seconds
        /// </summary>
        [JsonPropertyName("position")]
        public double Position { get; set; }

        /// <summary>
        /// Duration of break in seconds
        /// </summary>
        [JsonPropertyName("duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Duration { get; set; }

        /// <summary>
        /// Indicates whether the break is embedded in the main stream
        /// </summary>
        [JsonPropertyName("isEmbedded")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsEmbedded { get; set; }

        /// <summary>
        /// Whether a break was watched
        /// Marked as true when the break begins to play
        /// </summary>
        [JsonPropertyName("isWatched")]
        public bool IsWatched { get; set; }
    }
}