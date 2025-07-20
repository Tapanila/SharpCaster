using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents a track (audio, video, or text) within a media item
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.Track">Google Cast Track Documentation</see>
    public class Track
    {
        /// <summary>
        /// Unique identifier of the track within a MediaInfo object
        /// </summary>
        [JsonPropertyName("trackId")]
        public int TrackId { get; set; }

        /// <summary>
        /// The type of track (audio, video, or text)
        /// </summary>
        [JsonPropertyName("type")]
        public TrackType Type { get; set; }

        /// <summary>
        /// Optional custom application data
        /// </summary>
        [JsonPropertyName("customData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? CustomData { get; set; }

        /// <summary>
        /// Optional language tag (required for SUBTITLES)
        /// </summary>
        [JsonPropertyName("language")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Language { get; set; }

        /// <summary>
        /// Optional human-readable track name
        /// </summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        /// <summary>
        /// Optional text track subtype
        /// </summary>
        [JsonPropertyName("subtype")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackType? Subtype { get; set; }

        /// <summary>
        /// Optional track content identifier
        /// </summary>
        [JsonPropertyName("trackContentId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TrackContentId { get; set; }

        /// <summary>
        /// Optional MIME type of track content
        /// </summary>
        [JsonPropertyName("trackContentType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TrackContentType { get; set; }
    }
}