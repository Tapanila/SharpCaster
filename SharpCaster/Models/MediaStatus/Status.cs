using System.Collections.Generic;

namespace SharpCaster.Models.MediaStatus
{
    public class Status
    {
        public long mediaSessionId { get; set; }
        public int playbackRate { get; set; }
        public string playerState { get; set; }
        public double currentTime { get; set; }
        public int supportedMediaCommands { get; set; }
        public Volume volume { get; set; }
        public List<int> activeTrackIds { get; set; }
        public Media media { get; set; }
        public int currentItemId { get; set; }
        public List<Item> items { get; set; }
        public string repeatMode { get; set; }
    }
}