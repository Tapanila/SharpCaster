using System.Runtime.Serialization;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message with request identifier and session identifier
    /// </summary>
    [DataContract]
    public class MessageWithSession : MessageWithId
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}
