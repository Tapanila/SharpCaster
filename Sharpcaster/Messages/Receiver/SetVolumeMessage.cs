using Sharpcaster.Models;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    [DataContract]
    public class SetVolumeMessage : MessageWithId
    {
        [DataMember(Name = "volume")]
        public Volume Volume { get; set; }

    }
}
