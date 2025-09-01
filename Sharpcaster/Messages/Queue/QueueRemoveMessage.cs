using Sharpcaster.Messages.Media;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    /// <summary>
    /// A request to remove a list of items from the queue. If the remaining queue is empty, the media session will be terminated.
    /// </summary>
    public class QueueRemoveMessage : MediaSessionMessage
    {
        /// <summary>
        /// The list of media item IDs to remove. If any of the items does not exist it will be ignored.
        /// Duplicated item IDs will also be ignored. Must not be null or empty.
        /// </summary>
        [JsonPropertyName("itemIds")]
        public int[] ItemIds { get; set; } = null!;

    }
}
