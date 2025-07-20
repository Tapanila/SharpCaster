using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents the status of an ad break
    /// </summary>
    public class BreakStatus
    {
        /// <summary>
        /// Gets or sets the current break time position in seconds
        /// </summary>
        [JsonPropertyName("currentBreakTime")]
        public double CurrentBreakTime { get; set; }

        /// <summary>
        /// Gets or sets the current break clip time position in seconds
        /// </summary>
        [JsonPropertyName("currentBreakClipTime")]
        public double CurrentBreakClipTime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the break
        /// </summary>
        [JsonPropertyName("breakId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BreakId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the break clip
        /// </summary>
        [JsonPropertyName("breakClipId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BreakClipId { get; set; }

        /// <summary>
        /// Gets or sets when the break has finished
        /// </summary>
        [JsonPropertyName("whenSkippable")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? WhenSkippable { get; set; }
    }
}