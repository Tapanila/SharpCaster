using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharpCaster.Models.ChromecastStatus
{
    public class ChromecastStatus
    {
        [JsonProperty("applications")]
        public List<ChromecastApplication> Applications { get; set; }
        [JsonProperty("isActiveInput")]
        public bool IsActiveInput { get; set; }
        [JsonProperty("isStandBy")]
        public bool IsStandBy { get; set; }
        [JsonProperty("volume")]
        public Volume Volume { get; set; }
    }
}