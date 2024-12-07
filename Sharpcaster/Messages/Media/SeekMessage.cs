using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Seek message
    /// </summary>
    public class SeekMessage : MediaSessionMessage
    {
        /// <summary>
        /// Gets or sets the current time
        /// </summary>
        [JsonPropertyName("currentTime")]
        public double CurrentTime { get; set; }
    }
}
