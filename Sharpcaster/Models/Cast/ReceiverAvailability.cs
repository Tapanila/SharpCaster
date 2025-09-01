using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Receiver availability states from chrome.cast.ReceiverAvailability
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.ReceiverAvailability">Google Cast ReceiverAvailability Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<ReceiverAvailability>))]
    public enum ReceiverAvailability
    {
        /// <summary>
        /// Receiver is available
        /// </summary>
        AVAILABLE,

        /// <summary>
        /// Receiver is unavailable
        /// </summary>
        UNAVAILABLE
    }
}