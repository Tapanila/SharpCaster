using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Receiver actions from chrome.cast.ReceiverAction
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.ReceiverAction">Google Cast ReceiverAction Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<ReceiverAction>))]
    public enum ReceiverAction
    {
        /// <summary>
        /// Cast action
        /// </summary>
        CAST,

        /// <summary>
        /// Stop action
        /// </summary>
        STOP
    }
}