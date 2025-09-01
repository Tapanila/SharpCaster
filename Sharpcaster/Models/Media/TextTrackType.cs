using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Text track subtypes
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast.media#.TextTrackType">Google Cast TextTrackType Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<TextTrackType>))]
    public enum TextTrackType
    {
        /// <summary>
        /// Subtitles track
        /// </summary>
        SUBTITLES,

        /// <summary>
        /// Captions track
        /// </summary>
        CAPTIONS,

        /// <summary>
        /// Descriptions track
        /// </summary>
        DESCRIPTIONS,

        /// <summary>
        /// Chapters track
        /// </summary>
        CHAPTERS,

        /// <summary>
        /// Metadata track
        /// </summary>
        METADATA
    }
}