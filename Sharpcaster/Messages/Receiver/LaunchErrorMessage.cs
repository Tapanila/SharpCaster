using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Launch Error message
    /// </summary>
    [ReceptionMessage]
    public class LaunchErrorMessage : MessageWithId
    {
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
