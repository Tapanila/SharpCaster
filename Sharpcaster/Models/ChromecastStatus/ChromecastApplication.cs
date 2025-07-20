using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.ChromecastStatus
{
    public class ChromecastApplication
    {
        [JsonPropertyName("appId")]
        public string AppId { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("namespaces")]
        public Collection<Namespace> Namespaces { get; set; }

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }

        [JsonPropertyName("statusText")]
        public string StatusText { get; set; }

        [JsonPropertyName("transportId")]
        public string TransportId { get; set; }
    }
}