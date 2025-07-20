using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Receiver capabilities from chrome.cast.Capability
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.Capability">Google Cast Capability Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<Capability>))]
    public enum Capability
    {
        /// <summary>
        /// Audio input capability
        /// </summary>
        AUDIO_IN,

        /// <summary>
        /// Audio output capability
        /// </summary>
        AUDIO_OUT,

        /// <summary>
        /// Multizone group capability
        /// </summary>
        MULTIZONE_GROUP,

        /// <summary>
        /// Video input capability
        /// </summary>
        VIDEO_IN,

        /// <summary>
        /// Video output capability
        /// </summary>
        VIDEO_OUT
    }
}