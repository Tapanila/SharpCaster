using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// HLS video segment formats
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<HlsVideoSegmentFormat>))]
    public enum HlsVideoSegmentFormat
    {
        /// <summary>
        /// MPEG-2 transport stream
        /// </summary>
        MPEG2_TS,

        /// <summary>
        /// fMP4 (fragmented MP4)
        /// </summary>
        FMP4
    }
}