using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Sender platforms from chrome.cast.SenderPlatform
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.SenderPlatform">Google Cast SenderPlatform Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<SenderPlatform>))]
    public enum SenderPlatform
    {
        /// <summary>
        /// Android platform
        /// </summary>
        ANDROID,

        /// <summary>
        /// Chrome platform
        /// </summary>
        CHROME,

        /// <summary>
        /// iOS platform
        /// </summary>
        IOS
    }
}