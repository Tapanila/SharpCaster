using Sharpcaster.Messages;
using Sharpcaster.Models.Media;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media {

    [DataContract]
    public class QueueLoadMessage : MessageWithSession
    {
        [DataMember(Name = "items")]
        public Item[] Items { get; set; }
    }
  
}
