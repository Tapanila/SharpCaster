using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// The desired media player state after a seek operation is complete
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ResumeState>))]
    public enum ResumeState
    {
        /// <summary>
        /// Resume playback after seek
        /// </summary>
        PLAYBACK_START,

        /// <summary>
        /// Pause after seek
        /// </summary>
        PLAYBACK_PAUSE
    }
}