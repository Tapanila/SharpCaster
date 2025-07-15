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
        public string BookTitle { get; set; }

        [JsonPropertyName("chapterNumber")]
        public int? ChapterNumber { get; set; }

        [JsonPropertyName("chapterTitle")]
        public string ChapterTitle { get; set; }
    }
}
