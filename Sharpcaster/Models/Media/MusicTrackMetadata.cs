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
        public string SongName { get; set; }

        [JsonPropertyName("albumName")]
        public string AlbumName { get; set; }

        [JsonPropertyName("albumArtist")]
        public string AlbumArtist { get; set; }

        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        [JsonPropertyName("composer")]
        public string Composer { get; set; }

        [JsonPropertyName("trackNumber")]
        public int? TrackNumber { get; set; }

        [JsonPropertyName("discNumber")]
        public int? DiscNumber { get; set; }

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; }
    }
}
