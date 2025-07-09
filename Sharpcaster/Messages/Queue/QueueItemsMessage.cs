using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Queue;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    [ReceptionMessage]
    public class QueueItemsMessage : MediaSessionMessage
    {
        [JsonPropertyName("items")]
        public QueueItem[] Items { get; set; }
    }
}
