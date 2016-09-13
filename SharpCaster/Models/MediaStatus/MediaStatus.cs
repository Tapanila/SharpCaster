using System.Collections.Generic;
using Newtonsoft.Json;
using SharpCaster.JsonConverters;

namespace SharpCaster.Models.MediaStatus
{
    public class MediaStatus
    {
        //TODO this lowerCamelCase to UpperCamelCase could be done by serializer settings
        //TODO instead of these properties
        [JsonProperty("mediaSessionId")]
        public int MediaSessionId { get; set; }

        [JsonProperty("PlaybackRate")]
        public int PlaybackRate { get; set; }

        [JsonProperty("playerState")]
        [JsonConverter(typeof(PlayerStateEnumConverter))]
        public PlayerState PlayerState { get; set; }

        [JsonProperty("currentTime")]
        public double CurrentTime { get; set; }

        [JsonProperty("supportedMediaCommands")]
        public int SupportedMediaCommands { get; set; }

        [JsonProperty("volume")]
        public Volume Volume { get; set; }

        [JsonProperty("activeTrackIds")]
        public List<int> ActiveTrackIds { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("currentItemId")]
        public int CurrentItemId { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("repeatMode")]
        public string RepeatMode { get; set; }

        [JsonProperty("idleReason")]
        [JsonConverter(typeof(IdleReasonEnumConverter))]
        public IdleReason IdleReason { get; set; }
    }

    public enum PlayerState
    {
        Buffering,
        Idle,
        Paused,
        Playing
    }

    public enum IdleReason
    {
        NONE,
        CANCELLED,
        INTERRUPTED,
        FINISHED,
        ERROR
    }
}