using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Invalid request message
    /// </summary>
    [ReceptionMessage]
    public class InvalidRequestMessage : MessageWithId
    {
        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
