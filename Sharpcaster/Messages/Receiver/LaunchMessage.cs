using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Launch message
    /// </summary>
    public class LaunchMessage : MessageWithId
    {
        /// <summary>
        /// Gets or sets the application identifier
        /// </summary>
        [JsonPropertyName("appId")]
        public string ApplicationId { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; } = "en-US";
    }
}
