using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents media information for a media item
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.Media">Google Cast Media Documentation</see>
    public class Media
    {
        /// <summary>
        /// Gets or sets the content identifier
        /// </summary>
        [JsonPropertyName("contentId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentId { get; set; }

        /// <summary>
        /// Gets or sets the content URL
        /// </summary>
        [JsonPropertyName("contentUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the stream type
        /// </summary>
        [JsonPropertyName("streamType")]
        public StreamType StreamType { get; set; } = StreamType.Buffered;

        /// <summary>
        /// Gets or sets the content type (MIME type)
        /// </summary>
        [JsonPropertyName("contentType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MediaMetadata? Metadata { get; set; }

        /// <summary>
        /// Gets or sets the duration of the media in seconds
        /// </summary>
        [JsonPropertyName("duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the custom application data
        /// </summary>
        [JsonPropertyName("customData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? CustomData { get; set; }

        /// <summary>
        /// Gets or sets the media tracks (audio, video, text)
        /// </summary>
        [JsonPropertyName("tracks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Track[]? Tracks { get; set; }

        /// <summary>
        /// Gets or sets the text track style
        /// </summary>
        [JsonPropertyName("textTrackStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextTrackStyle? TextTrackStyle { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier for deep linking
        /// </summary>
        [JsonPropertyName("entity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Entity { get; set; }

        /// <summary>
        /// Gets or sets the Android TV entity identifier
        /// </summary>
        [JsonPropertyName("atvEntity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AtvEntity { get; set; }

        /// <summary>
        /// Gets or sets the start time for live streams
        /// </summary>
        [JsonPropertyName("startAbsoluteTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? StartAbsoluteTime { get; set; }

        /// <summary>
        /// Gets or sets the break clips for ad breaks
        /// </summary>
        [JsonPropertyName("breakClips")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BreakClip[]? BreakClips { get; set; }

        /// <summary>
        /// Gets or sets the breaks for ad scheduling
        /// </summary>
        [JsonPropertyName("breaks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Break[]? Breaks { get; set; }

        /// <summary>
        /// Gets or sets the HLS segment format
        /// </summary>
        [JsonPropertyName("hlsSegmentFormat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HlsSegmentFormat? HlsSegmentFormat { get; set; }

        /// <summary>
        /// Gets or sets the HLS video segment format
        /// </summary>
        [JsonPropertyName("hlsVideoSegmentFormat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HlsVideoSegmentFormat? HlsVideoSegmentFormat { get; set; }

        /// <summary>
        /// Gets or sets the VMAP ads request
        /// </summary>
        [JsonPropertyName("vmapAdsRequest")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VastAdsRequest? VmapAdsRequest { get; set; }
    }
}
