using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Queue
{

    //TODO: not tested yet. Either implement Queue manipulation Requests or test with 2nd external client!?

    [DataContract]
    [ReceptionMessage]
    public class QueueChangeMessage : MessageWithSession
    {
        [DataMember(Name = "changeType")]
        public string ChangeType { get; set; }

        [DataMember(Name = "itemIds")]
        public int[] ChangedIds { get; set; }
    }


}
