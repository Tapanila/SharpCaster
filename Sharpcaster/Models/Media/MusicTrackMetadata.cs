using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    public class MusicTrackMetadata : MediaMetadata
    {
        public MusicTrackMetadata()
        {
            MetadataType = MetadataType.Music;
        }

        [JsonPropertyName("songName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SongName { get; set; }

        [JsonPropertyName("albumName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AlbumName { get; set; }

        [JsonPropertyName("albumArtist")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AlbumArtist { get; set; }

        [JsonPropertyName("artist")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Artist { get; set; }

        [JsonPropertyName("composer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Composer { get; set; }

        [JsonPropertyName("trackNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TrackNumber { get; set; }

        [JsonPropertyName("discNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? DiscNumber { get; set; }

        [JsonPropertyName("releaseDate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReleaseDate { get; set; }
    }
}
