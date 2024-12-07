using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models
{
    public class Volume
    {
        [JsonPropertyName("level")]
        public double? Level { get; set; }

        [JsonPropertyName("muted")]
        public bool? Muted { get; set; }
    }
}
