using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents a break clip, typically used for ads during media playback
    /// </summary>
    public class BreakClip
    {
        /// <summary>
        /// Unique ID of break clip
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// URL or content ID of break media
        /// </summary>
        [JsonPropertyName("contentId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentId { get; set; }

        /// <summary>
        /// Content MIME type
        /// </summary>
        [JsonPropertyName("contentType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentType { get; set; }

        /// <summary>
        /// Media URL, used if different from contentId
        /// </summary>
        [JsonPropertyName("contentUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentUrl { get; set; }

        /// <summary>
        /// Break clip duration in seconds
        /// </summary>
        [JsonPropertyName("duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Duration { get; set; }

        /// <summary>
        /// Break clip title for sender display
        /// </summary>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        /// <summary>
        /// Content URL to display while clip plays
        /// </summary>
        [JsonPropertyName("posterUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PosterUrl { get; set; }

        /// <summary>
        /// URL for sender UI link
        /// </summary>
        [JsonPropertyName("clickThroughUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ClickThroughUrl { get; set; }

        /// <summary>
        /// Time in seconds when clip becomes skippable
        /// </summary>
        [JsonPropertyName("whenSkippable")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? WhenSkippable { get; set; }

        /// <summary>
        /// VAST ad request configuration
        /// </summary>
        [JsonPropertyName("vastAdsRequest")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VastAdsRequest? VastAdsRequest { get; set; }

        /// <summary>
        /// Application-specific break clip data
        /// </summary>
        [JsonPropertyName("customData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? CustomData { get; set; }

        /// <summary>
        /// Format of HLS media segment
        /// </summary>
        [JsonPropertyName("hlsSegmentFormat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HlsSegmentFormat? HlsSegmentFormat { get; set; }
    }
}