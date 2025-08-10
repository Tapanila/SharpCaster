using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    public class AudiobookChapterMetadata : MediaMetadata
    {
        public AudiobookChapterMetadata()
        {
            MetadataType = MetadataType.AudiobookChapter;
        }

        [JsonPropertyName("bookTitle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BookTitle { get; set; }

        [JsonPropertyName("chapterNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ChapterNumber { get; set; }

        [JsonPropertyName("chapterTitle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChapterTitle { get; set; }
    }
}
