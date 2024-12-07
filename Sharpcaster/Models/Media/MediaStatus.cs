using System.Text.Json.Serialization;
using Sharpcaster.Converters;
using Sharpcaster.Models.Queue;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media status
    /// </summary>
    public class MediaStatus
    {
        /// <summary>
        /// Gets or sets the media session identifier
        /// </summary>
        [JsonPropertyName("mediaSessionId")]
        public long MediaSessionId { get; set; }

        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        [JsonPropertyName("playbackRate")]
        public int PlaybackRate { get; set; }

        /// <summary>
        /// Gets or sets the player state
        /// </summary>
        [JsonPropertyName("playerState")]
        [JsonConverter(typeof(PlayerStateEnumConverter))]
        public PlayerStateType PlayerState { get; set; }

        /// <summary>
        /// Gets or sets the current time
        /// </summary>
        [JsonPropertyName("currentTime")]
        public double CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets the supported media commands
        /// </summary>
        [JsonPropertyName("supportedMediaCommands")]
        public int SupportedMediaCommands { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [JsonPropertyName("volume")]
        public Volume Volume { get; set; }

        /// <summary>
        /// Gets or sets the idle reason
        /// </summary>
        [JsonPropertyName("idleReason")]
        public string IdleReason { get; set; }

        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [JsonPropertyName("media")]
        public Media Media { get; set; }

        /// <summary>
        /// Gets or sets the current item identifier
        /// </summary>
        [JsonPropertyName("currentItemId")]
        public int CurrentItemId { get; set; } = -1;

        /// <summary>
        /// Gets or sets the extended status
        /// </summary>
        [JsonPropertyName("extendedStatus")]
        public MediaStatus ExtendedStatus { get; set; }

        /// <summary>
        /// Gets or sets the repeat mode
        /// </summary>
        [JsonPropertyName("repeatMode")]
        [JsonConverter(typeof(RepeatModeEnumConverter))]
        public RepeatModeType RepeatMode { get; set; }

        /// <summary>
        /// Gets or sets the queue data
        /// </summary>
        [JsonPropertyName("queueData")]
        public QueueData QueueData { get; set; }

        /// <summary>
        /// Gets or sets the queue items
        /// </summary>
        [JsonPropertyName("items")]
        public QueueItem[] Items { get; set; }
    }
}