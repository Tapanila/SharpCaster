using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Media;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{

    [DataContract]
    [ReceptionMessage]
    public class QueueItemsMessage : MediaSessionMessage
    {

        [DataMember(Name = "items")]
        public Item[] Items { get; set; }
    }
}
