using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// HLS (HTTP Live Streaming) segment formats
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<HlsSegmentFormat>))]
    public enum HlsSegmentFormat
    {
        /// <summary>
        /// AAC audio in MPEG-2 transport stream
        /// </summary>
        AAC,

        /// <summary>
        /// AC-3 audio
        /// </summary>
        AC3,

        /// <summary>
        /// MP3 audio
        /// </summary>
        MP3,

        /// <summary>
        /// MPEG-2 transport stream
        /// </summary>
        TS,

        /// <summary>
        /// MPEG-2 transport stream with AAC audio
        /// </summary>
        TS_AAC,

        /// <summary>
        /// Enhanced AC-3 audio
        /// </summary>
        E_AC3,

        /// <summary>
        /// fMP4 (fragmented MP4)
        /// </summary>
        FMP4
    }
}