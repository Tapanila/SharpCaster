using Sharpcaster.Messages;
using Sharpcaster.Models.Media;
using System.Runtime.Serialization;

namespace Sharpcaster.queue.messages {

    [DataContract]
    public class QueueLoadMessage : MessageWithSession
    {
        [DataMember(Name = "items")]
        public Item[] Items { get; set; }
    }
  
}
