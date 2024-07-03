using Newtonsoft.Json;
using Sharpcaster.Converters;
using Sharpcaster.Models.Queue;
using System.Runtime.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media status
    /// </summary>
    [DataContract]
    public class MediaStatus
    {
        /// <summary>
        /// Gets or sets the media session identifier
        /// </summary>
        [DataMember(Name = "mediaSessionId")]
        public long MediaSessionId { get; set; }

        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        [DataMember(Name = "playbackRate")]
        public int PlaybackRate { get; set; }

        /// <summary>
        /// Gets or sets the player state
        /// </summary>
        [DataMember(Name = "playerState")]
        [JsonConverter(typeof(PlayerStateEnumConverter))]
        public PlayerStateType PlayerState { get; set; }

        /// <summary>
        /// Gets or sets the current time
        /// </summary>
        [DataMember(Name = "currentTime")]
        public double CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets the supported media commands
        /// </summary>
        [DataMember(Name = "supportedMediaCommands")]
        public int SupportedMediaCommands { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [DataMember(Name = "volume")]
        public Volume Volume { get; set; }

        /// <summary>
        /// Gets or sets the idle reason
        /// </summary>
        [DataMember(Name = "idleReason")]
        public string IdleReason { get; set; }

        /// <summary>
        /// Gets or sets the media
        /// </summary>
        [DataMember(Name = "media")]
        public Media Media { get; set; }

        /// <summary>
        /// Gets or sets the current item identifier
        /// </summary>
        [DataMember(Name = "currentItemId")]
        public int CurrentItemId { get; set; } = -1;

        /// <summary>
        /// Gets or sets the extended status
        /// </summary>
        [DataMember(Name = "extendedStatus")]
        public MediaStatus ExtendedStatus { get; set; }

        /// <summary>
        /// Gets or sets the repeat mode
        /// </summary>
        [DataMember(Name = "repeatMode")]
        public string RepeatMode { get; set; }

        [DataMember(Name = "queueData")]
        public QueueData QueueData { get; set; }


        [DataMember(Name = "items")]
        public QueueItem[] Items { get; set; }

    }
}
