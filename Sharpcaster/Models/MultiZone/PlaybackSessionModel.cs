using System.Text.Json.Serialization;

namespace Sharpcaster.Models.MultiZone
{
    public class PlaybackSessionModel
    {
        [JsonPropertyName("appAllowsGrouping")]
        public bool? AppAllowsGrouping { get; set; }

        [JsonPropertyName("isVideoContent")]
        public bool? IsVideoContent { get; set; }

        [JsonPropertyName("streamTransferSupported")]
        public bool? StreamTransferSupported { get; set; }
    }
}
