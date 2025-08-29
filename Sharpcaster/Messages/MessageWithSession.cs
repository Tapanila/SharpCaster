using System.Text.Json.Serialization;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message with request identifier and session identifier
    /// </summary>
    public class MessageWithSession : MessageWithId
    {
        [JsonPropertyName("sessionId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SessionId { get; set; }
    }
}
