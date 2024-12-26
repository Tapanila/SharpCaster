using System.Text.Json.Serialization;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message with request identifier and session identifier
    /// </summary>
    public class MessageWithSession : MessageWithId
    {
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
    }
}
