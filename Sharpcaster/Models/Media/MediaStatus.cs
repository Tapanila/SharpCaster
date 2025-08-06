using System.Text.Json.Serialization;
using Sharpcaster.Converters;
using Sharpcaster.Models.Queue;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media status
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.MediaStatus">Google Cast MediaStatus Documentation</see>
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
        public double PlaybackRate { get; set; }

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
        [JsonConverter(typeof(MediaCommandEnumConverter))]
        public MediaCommand SupportedMediaCommands { get; set; }

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
        /// Only contains two queue items: the current item and the next item
        /// </summary>
        [JsonPropertyName("queueData")]
        public QueueData QueueData { get; set; }

        /// <summary>
        /// Gets or sets the queue items
        /// </summary>
        [JsonPropertyName("items")]
        public QueueItem[] Items { get; set; }

        /// <summary>
        /// Gets or sets the active track IDs
        /// </summary>
        [JsonPropertyName("activeTrackIds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? ActiveTrackIds { get; set; }

        /// <summary>
        /// Gets or sets the loading item ID
        /// </summary>
        [JsonPropertyName("loadingItemId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? LoadingItemId { get; set; }

        /// <summary>
        /// Gets or sets the preloaded item ID
        /// </summary>
        [JsonPropertyName("preloadedItemId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PreloadedItemId { get; set; }

        /// <summary>
        /// Gets or sets the live seekable range
        /// </summary>
        [JsonPropertyName("liveSeekableRange")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LiveSeekableRange? LiveSeekableRange { get; set; }

        /// <summary>
        /// Gets or sets the video information
        /// </summary>
        [JsonPropertyName("videoInfo")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VideoInformation? VideoInfo { get; set; }

        /// <summary>
        /// Gets or sets the break status
        /// </summary>
        [JsonPropertyName("breakStatus")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BreakStatus? BreakStatus { get; set; }

        /// <summary>
        /// Gets or sets custom data
        /// </summary>
        [JsonPropertyName("customData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? CustomData { get; set; }
    }
}