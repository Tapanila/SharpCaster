using System.Text.Json.Serialization;

namespace Sharpcaster.Models.MultiZone
{
    public class MultiZoneStatus
    {
        [JsonPropertyName("devices")]
        public Device[] Devices { get; set; }

        [JsonPropertyName("isMultichannel")]
        public bool? IsMultichannel { get; set; }

        [JsonPropertyName("playbackSession")]
        public PlaybackSessionModel PlaybackSession { get; set; }
    }
}
