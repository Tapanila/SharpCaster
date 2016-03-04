using System.Collections.Generic;
using Newtonsoft.Json;
using SharpCaster.JsonConverters;

namespace SharpCaster.Models.MediaStatus
{
    public class MediaStatus
    {
        public long mediaSessionId { get; set; }
        public int playbackRate { get; set; }
        [JsonProperty("playerState")]
        [JsonConverter(typeof(PlayerStateEnumConverter))]
        public PlayerState PlayerState { get; set; }
        public double currentTime { get; set; }
        public int supportedMediaCommands { get; set; }
        public Volume volume { get; set; }
        public List<int> activeTrackIds { get; set; }
        public Media media { get; set; }
        public int currentItemId { get; set; }
        public List<Item> items { get; set; }
        public string repeatMode { get; set; }
    }

    public enum PlayerState
    {
        Buffering,
        Idle,
        Paused,
        Playing
    }
}