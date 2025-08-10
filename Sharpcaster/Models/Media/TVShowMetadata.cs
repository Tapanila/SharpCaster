using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    public class TVShowMetadata : MediaMetadata
    {
        public TVShowMetadata()
        {
            MetadataType = MetadataType.TVShow;
        }

        [JsonPropertyName("seriesTitle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SeriesTitle { get; set; }

        [JsonPropertyName("episode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Episode { get; set; }

        [JsonPropertyName("season")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Season { get; set; }

        [JsonPropertyName("originalAirdate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OriginalAirdate { get; set; }
    }
}
