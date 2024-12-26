using System.Text.Json.Serialization;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for a message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the message type
        /// </summary>
        [JsonPropertyName("type")]
        string Type { get; }
    }
}
