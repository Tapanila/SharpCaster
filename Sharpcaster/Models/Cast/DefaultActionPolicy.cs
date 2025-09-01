using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Default action policies from chrome.cast.DefaultActionPolicy
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.DefaultActionPolicy">Google Cast DefaultActionPolicy Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<DefaultActionPolicy>))]
    public enum DefaultActionPolicy
    {
        /// <summary>
        /// Cast this tab
        /// </summary>
        CAST_THIS_TAB,

        /// <summary>
        /// Create session
        /// </summary>
        CREATE_SESSION
    }
}