using System.Text.Json.Serialization;
using Sharpcaster.Models.Queue;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Load message
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.LoadRequest">Google Cast LoadRequest Documentation</see>
    public class LoadMessage : MessageWithSession
    {
        /// <summary>
        /// Array of Track trackIds that should be active
        /// </summary>
        [JsonPropertyName("activeTrackIds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? ActiveTrackIds { get; set; }

        /// <summary>
        /// Alternate Android TV credentials
        /// </summary>
        [JsonPropertyName("atvCredentials")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AtvCredentials { get; set; }

        /// <summary>
        /// Alternate Android TV credentials type
        /// </summary>
        [JsonPropertyName("atvCredentialsType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AtvCredentialsType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the media must be played directly or not
        /// </summary>
        [JsonPropertyName("autoplay")]
        public bool AutoPlay { get; set; } = true;

        /// <summary>
        /// Optional user credentials
        /// </summary>
        [JsonPropertyName("credentials")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Credentials { get; set; }

        /// <summary>
        /// Optional credentials type
        /// </summary>
        [JsonPropertyName("credentialsType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CredentialsType { get; set; }

        /// <summary>
        /// Seconds from the beginning of the media to start playback
        /// </summary>
        [JsonPropertyName("currentTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [JsonPropertyName("media")]
        public Models.Media.Media Media { get; set; } = null!;

        /// <summary>
        /// The media playback rate
        /// </summary>
        [JsonPropertyName("playbackRate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? PlaybackRate { get; set; }

        /// <summary>
        /// Queue data
        /// </summary>
        [JsonPropertyName("queueData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public QueueData? QueueData { get; set; }
    }
}
