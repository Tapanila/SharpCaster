using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Set playback rate message
    /// </summary>
    public class SetPlaybackRateMessage : MediaSessionMessage
    {
        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        [JsonPropertyName("playbackRate")]
        public double PlaybackRate { get; set; }
    }
}