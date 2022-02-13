using Newtonsoft.Json;

namespace Sharpcaster.Models.ChromecastStatus
{
    public class Namespace
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
