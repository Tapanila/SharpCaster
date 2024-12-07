using System.Text.Json.Serialization;

namespace Sharpcaster.Models.ChromecastStatus
{
    public class Namespace
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}