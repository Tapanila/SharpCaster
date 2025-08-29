using Sharpcaster.Messages.Media;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    public class QueueGetItemsMessage : MediaSessionMessage
    {
        [JsonPropertyName("itemIds")]
        public int[]? Ids { get; set; }
    }
}