using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages
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
