using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    /// <summary>
    /// Launch Error message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    class LaunchErrorMessage : MessageWithId
    {
       
        [DataMember(Name = "reason")]
        public string Reason { get; set; }
    }
}
