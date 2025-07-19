using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    public class PhotoMetadata : MediaMetadata
    {
        public PhotoMetadata()
        {
            MetadataType = MetadataType.Photo;
        }

        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        [JsonPropertyName("creationDateTime")]
        public string CreationDateTime { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}
