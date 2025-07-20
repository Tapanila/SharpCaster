using System.Text.Json.Serialization;
using Sharpcaster.Models.Cast;

namespace Sharpcaster.Models
{
    /// <summary>
    /// Represents the volume settings for a device or media stream
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.Volume">Google Cast Volume Documentation</see>
    public class Volume
    {
        /// <summary>
        /// The type of volume control that is available
        /// </summary>
        [JsonPropertyName("controlType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VolumeControlType? ControlType { get; set; }

        /// <summary>
        /// The current volume level as a value between 0.0 and 1.0
        /// </summary>
        [JsonPropertyName("level")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Level { get; set; }

        /// <summary>
        /// Whether the receiver is muted, independent of the volume level
        /// </summary>
        [JsonPropertyName("muted")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Muted { get; set; }

        /// <summary>
        /// The allowed steps for changing volume
        /// </summary>
        [JsonPropertyName("stepInterval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? StepInterval { get; set; }
    }
}
