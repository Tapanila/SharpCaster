using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Spotify
{
    public class AddUserMessage : MessageWithId
    {
        [JsonPropertyName("payload")]
        public AddUserMessagePayload Payload { get; set; }
        public AddUserMessage()
        {
            Type = "addUser";
        }
    }

    public class AddUserMessagePayload
    {
        [JsonPropertyName("blob")]
        public string Blob { get; set; }
        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; }
    }
}
