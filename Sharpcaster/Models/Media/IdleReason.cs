using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Reason why the player is in IDLE state
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<IdleReason>))]
    public enum IdleReason
    {
        /// <summary>
        /// A sender requested to stop playback using the STOP command
        /// </summary>
        CANCELLED,

        /// <summary>
        /// A sender requested playing a different media
        /// </summary>
        INTERRUPTED,

        /// <summary>
        /// The media playback completed
        /// </summary>
        FINISHED,

        /// <summary>
        /// The media was interrupted due to networking errors
        /// </summary>
        ERROR
    }
}