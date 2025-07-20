using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Queue;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    /// <summary>
    /// A request to insert a list of new media items into the queue.
    /// </summary>
    public class QueueInsertMessage : MediaSessionMessage
    {
        /// <summary>
        /// List of queue items to insert. The itemId field of the items should be empty or the request will fail with an INVALID_PARAMS error.
        /// It is sorted (first element will be played first).
        /// </summary>
        [JsonPropertyName("items")]
        public QueueItem[] Items { get; set; }


        /// <summary>
        /// ID of the item that will be located immediately after the inserted list. If null or not found the list will be appended to the end of the queue.
        /// </summary>
        [JsonPropertyName("insertBefore")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InsertBefore { get; set; }
    }
}
