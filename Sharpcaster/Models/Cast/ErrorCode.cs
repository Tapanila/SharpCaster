using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Error codes from chrome.cast.ErrorCode
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.ErrorCode">Google Cast ErrorCode Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<ErrorCode>))]
    public enum ErrorCode
    {
        /// <summary>
        /// API not initialized
        /// </summary>
        API_NOT_INITIALIZED,

        /// <summary>
        /// Operation was cancelled
        /// </summary>
        CANCEL,

        /// <summary>
        /// Channel error occurred
        /// </summary>
        CHANNEL_ERROR,

        /// <summary>
        /// Cast extension is missing
        /// </summary>
        EXTENSION_MISSING,

        /// <summary>
        /// Cast extension is not compatible
        /// </summary>
        EXTENSION_NOT_COMPATIBLE,

        /// <summary>
        /// Invalid parameter provided
        /// </summary>
        INVALID_PARAMETER,

        /// <summary>
        /// Failed to load media
        /// </summary>
        LOAD_MEDIA_FAILED,

        /// <summary>
        /// Receiver is unavailable
        /// </summary>
        RECEIVER_UNAVAILABLE,

        /// <summary>
        /// Session error occurred
        /// </summary>
        SESSION_ERROR,

        /// <summary>
        /// Operation timed out
        /// </summary>
        TIMEOUT
    }
}