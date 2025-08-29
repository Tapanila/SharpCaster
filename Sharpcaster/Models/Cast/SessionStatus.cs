using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Session connection status from chrome.cast.SessionStatus
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.SessionStatus">Google Cast SessionStatus Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<SessionStatus>))]
    public enum SessionStatus
    {
        /// <summary>
        /// Session is connected
        /// </summary>
        CONNECTED,

        /// <summary>
        /// Session is disconnected
        /// </summary>
        DISCONNECTED,

        /// <summary>
        /// Session is stopped
        /// </summary>
        STOPPED
    }
}