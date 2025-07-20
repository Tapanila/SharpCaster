using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Media commands that can be supported by the media player
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<MediaCommand>))]
    public enum MediaCommand
    {
        /// <summary>
        /// Pause command
        /// </summary>
        PAUSE,

        /// <summary>
        /// Seek command
        /// </summary>
        SEEK,

        /// <summary>
        /// Stream volume command
        /// </summary>
        STREAM_VOLUME,

        /// <summary>
        /// Stream mute command
        /// </summary>
        STREAM_MUTE,

        /// <summary>
        /// Skip ad command
        /// </summary>
        SKIP_AD
    }
}