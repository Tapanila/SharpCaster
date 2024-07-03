using Sharpcaster.Messages.Media;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{
    [DataContract]
    [ReceptionMessage]
    public class QueueItemIdsMessage : MediaSessionMessage
    {

        [DataMember(Name = "itemIds")]
        public int[] Ids { get; set; }
    }
}
