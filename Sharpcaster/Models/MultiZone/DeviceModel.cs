using System.Text.Json.Serialization;

namespace Sharpcaster.Models.MultiZone
{
    public class Device
    {
        [JsonPropertyName("capabilities")]
        public string Capabilities { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [JsonPropertyName("volume")]
        public Volume Volume { get; set; }
    }
}
