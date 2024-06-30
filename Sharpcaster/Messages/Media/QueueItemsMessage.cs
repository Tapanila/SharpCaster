
using Sharpcaster.Messages;
using Sharpcaster.Messages.Media;
using Sharpcaster.Models.Media;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media {

    [DataContract]
    [ReceptionMessage]
    public class QueueItemsMessage : MediaSessionMessage {

        [DataMember(Name = "items")]
        public Item[] Items { get; set; }
    }
}
