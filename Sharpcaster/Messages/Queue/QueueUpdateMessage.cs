using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Queue;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{
    /// <summary>
    /// A request to update properties of the existing items in the media queue.
    /// </summary>
    public class QueueUpdateMessage : MediaSessionMessage
    {
        /// <summary>
        /// List of queue items to be updated. No reordering will happen, the items will retain the existing order and will be fully replaced with the ones provided,
        /// including the media information. The items not provided in this list will remain unchanged.
        /// The tracks information can not change once the item is loaded (if the item is the currentItem). If any of the items does not exist it will be ignored.
        /// </summary>
        [DataMember(Name = "items")]
        public QueueItem[] Items { get; set; }

    }
}
