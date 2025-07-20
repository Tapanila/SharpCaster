using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Stream transfer message
    /// </summary>
    public class StreamTransferMessage : MediaSessionMessage
    {
        /// <summary>
        /// Gets or sets the transfer request
        /// </summary>
        [JsonPropertyName("transferRequest")]
        public object TransferRequest { get; set; }
    }
}