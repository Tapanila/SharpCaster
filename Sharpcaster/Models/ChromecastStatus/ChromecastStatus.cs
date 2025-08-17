using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.ChromecastStatus
{
    /// <summary>
    /// Represents the current status of a Chromecast device
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.ReceiverStatus">Google Cast ReceiverStatus Documentation</see>
    public class ChromecastStatus
    {
        [JsonPropertyName("applications")]
        public Collection<ChromecastApplication>? Applications { get; set; }

        [JsonPropertyName("isActiveInput")]
        public bool IsActiveInput { get; set; }

        [JsonPropertyName("isStandBy")]
        public bool IsStandBy { get; set; }

        [JsonPropertyName("volume")]
        public Volume? Volume { get; set; }

        [JsonIgnore]
        public ChromecastApplication? Application => Applications?.FirstOrDefault();
    }
}