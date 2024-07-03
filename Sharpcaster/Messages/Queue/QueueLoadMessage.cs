using Sharpcaster.Models.Queue;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{
    [DataContract]
    public class QueueLoadMessage : MessageWithSession
    {
        [DataMember(Name = "items")]
        public QueueItem[] Items { get; set; }
    }
}
