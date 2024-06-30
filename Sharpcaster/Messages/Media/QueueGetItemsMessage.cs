using Sharpcaster.Messages.Media;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media {

    [DataContract]
    public class QueueGetItemsMessage : MediaSessionMessage {
        [DataMember(Name = "itemIds")]
        public int[] Ids { get; set; }
    }


}