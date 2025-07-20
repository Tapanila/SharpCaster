using Sharpcaster.Messages.Media;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Queue
{
    /// <summary>
    /// A request to reorder a list of media items in the queue.
    /// </summary>
    public class QueueReorderMessage : MediaSessionMessage
    {
        /// <summary>
        /// The list of media item IDs to reorder, in the new order. Items not provided will keep their existing order (without the items being reordered).
        /// The provided list will be inserted at the position determined by insertBefore.
        /// 
        /// For example:
        /// If insertBefore is not specified Existing queue: “A”,”D”,”G”,”H”,”B”,”E” itemIds: “D”,”H”,”B” New Order: “A”,”G”,”E”,“D”,”H”,”B”
        /// If insertBefore is “A” Existing queue: “A”,”D”,”G”,”H”,”B” itemIds: “D”,”H”,”B” New Order: “D”,”H”,”B”,“A”,”G”,”E”
        /// If insertBefore is “G” Existing queue: “A”,”D”,”G”,”H”,”B” itemIds: “D”,”H”,”B” New Order: “A”,“D”,”H”,”B”,”G”,”E”
        /// 
        /// If any of the items does not exist it will be ignored. Must not be null or empty.
        /// </summary>
        [JsonPropertyName("itemIds")]
        public int[] ItemIds { get; set; } = null!;

        /// <summary>
        /// ID of the item that will be located immediately after the reordered list.
        /// If null or not found, the reordered list will be appended at the end of the queue.
        /// This ID can not be one of the IDs in the itemIds list.
        /// </summary>
        [JsonPropertyName("insertBefore")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InsertBefore { get; set; }
    }
}
