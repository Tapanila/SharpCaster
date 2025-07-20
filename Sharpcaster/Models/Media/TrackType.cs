using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Track types for media
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.TrackType">Google Cast TrackType Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<TrackType>))]
    public enum TrackType
    {
        /// <summary>
        /// Text track (subtitles, captions, etc.)
        /// </summary>
        TEXT,

        /// <summary>
        /// Audio track
        /// </summary>
        AUDIO,

        /// <summary>
        /// Video track
        /// </summary>
        VIDEO
    }
}