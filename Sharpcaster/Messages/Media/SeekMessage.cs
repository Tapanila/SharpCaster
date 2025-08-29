using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Seek message
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.SeekRequest">Google Cast SeekRequest Documentation</see>
    public class SeekMessage : MediaSessionMessage
    {
        /// <summary>
        /// Gets or sets the current time
        /// </summary>
        [JsonPropertyName("currentTime")]
        public double CurrentTime { get; set; }

        /// <summary>
        /// Resume state after the seek operation completes
        /// </summary>
        [JsonPropertyName("resumeState")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Models.Media.ResumeState? ResumeState { get; set; }
    }
}
