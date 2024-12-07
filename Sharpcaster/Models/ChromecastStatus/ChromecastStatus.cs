using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.ChromecastStatus
{
    public class ChromecastStatus
    {
        [JsonPropertyName("applications")]
        public List<ChromecastApplication> Applications { get; set; }

        [JsonPropertyName("isActiveInput")]
        public bool IsActiveInput { get; set; }

        [JsonPropertyName("isStandBy")]
        public bool IsStandBy { get; set; }

        [JsonPropertyName("volume")]
        public Volume Volume { get; set; }

        [JsonIgnore]
        public ChromecastApplication Application => Applications?.FirstOrDefault();
    }
}