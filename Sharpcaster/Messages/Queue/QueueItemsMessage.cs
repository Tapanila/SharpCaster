using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Queue;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{

    [DataContract]
    [ReceptionMessage]
    public class QueueItemsMessage : MediaSessionMessage
    {

        [DataMember(Name = "items")]
        public QueueItem[] Items { get; set; }
    }
}
