using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    public class LaunchStatusMessage : MessageWithId
    {
        [JsonPropertyName("launchRequestId")]
        public int LaunchRequestId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
