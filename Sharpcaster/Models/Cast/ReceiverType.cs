using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Types of receiver devices from chrome.cast.ReceiverType
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.ReceiverType">Google Cast ReceiverType Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<ReceiverType>))]
    public enum ReceiverType
    {
        /// <summary>
        /// Cast receiver
        /// </summary>
        CAST,

        /// <summary>
        /// Custom receiver
        /// </summary>
        CUSTOM,

        /// <summary>
        /// DIAL receiver
        /// </summary>
        DIAL,

        /// <summary>
        /// Hangout receiver
        /// </summary>
        HANGOUT
    }
}