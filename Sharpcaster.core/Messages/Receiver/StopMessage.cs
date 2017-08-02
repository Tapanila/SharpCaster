using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages.Receiver
{

    [DataContract]
    public class StopMessage : MessageWithId
    {   
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}
