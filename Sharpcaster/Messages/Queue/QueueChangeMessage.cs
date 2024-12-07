using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    [ReceptionMessage]
    public class QueueChangeMessage : MessageWithSession
    {
        [JsonPropertyName("changeType")]
        public string ChangeType { get; set; }

        [JsonPropertyName("itemIds")]
        public int[] ChangedIds { get; set; }
    }
}
