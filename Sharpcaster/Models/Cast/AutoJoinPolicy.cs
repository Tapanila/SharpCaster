using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Auto-join policies from chrome.cast.AutoJoinPolicy
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.AutoJoinPolicy">Google Cast AutoJoinPolicy Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<AutoJoinPolicy>))]
    public enum AutoJoinPolicy
    {
        /// <summary>
        /// Scoped to origin
        /// </summary>
        ORIGIN_SCOPED,

        /// <summary>
        /// Scoped to page
        /// </summary>
        PAGE_SCOPED,

        /// <summary>
        /// Scoped to tab and origin
        /// </summary>
        TAB_AND_ORIGIN_SCOPED
    }
}