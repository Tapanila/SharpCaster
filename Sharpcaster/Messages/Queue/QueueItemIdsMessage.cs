using Sharpcaster.Messages.Media;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    [ReceptionMessage]
    public class QueueItemIdsMessage : MediaSessionMessage
    {
        [JsonPropertyName("itemIds")]
        public int[] Ids { get; set; }
    }
}
