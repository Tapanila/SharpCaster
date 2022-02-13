using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sharpcaster.Models.ChromecastStatus
{
    public class ChromecastApplication
    {
        [JsonProperty("appId")]
        public string AppId { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("namespaces")]
        public List<Namespace> Namespaces { get; set; }
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("statusText")]
        public string StatusText { get; set; }
        [JsonProperty("transportId")]
        public string TransportId { get; set; }
    }
}
