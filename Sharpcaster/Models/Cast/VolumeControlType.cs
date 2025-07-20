using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Cast
{
    /// <summary>
    /// Volume control types from chrome.cast.VolumeControlType
    /// </summary>
    /// <see href="https://developers.google.com/cast/docs/reference/web_sender/chrome.cast#.VolumeControlType">Google Cast VolumeControlType Documentation</see>
    [JsonConverter(typeof(JsonStringEnumConverter<VolumeControlType>))]
    public enum VolumeControlType
    {
        /// <summary>
        /// Volume can be attenuated
        /// </summary>
        ATTENUATION,

        /// <summary>
        /// Volume is fixed
        /// </summary>
        FIXED,

        /// <summary>
        /// Volume is master controlled
        /// </summary>
        MASTER
    }
}