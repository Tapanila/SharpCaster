using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Types of container objects for media
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ContainerType>))]
    public enum ContainerType
    {
        /// <summary>
        /// Generic container type
        /// </summary>
        GENERIC_CONTAINER,

        /// <summary>
        /// Audiobook container
        /// </summary>
        AUDIOBOOK_CONTAINER,

        /// <summary>
        /// Live TV channel container
        /// </summary>
        LIVE_TV_CONTAINER
    }
}