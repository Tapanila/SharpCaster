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
        public string SeriesTitle { get; set; }

        [JsonPropertyName("episode")]
        public int? Episode { get; set; }

        [JsonPropertyName("season")]
        public int? Season { get; set; }

        [JsonPropertyName("originalAirdate")]
        public string OriginalAirdate { get; set; }
    }
}
