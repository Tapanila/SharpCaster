using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sharpcaster.Messages.Receiver
{
    [DataContract]
    public class LaunchStatusMessage : MessageWithId
    {
        [DataMember(Name = "launchRequestId")]
        public string LaunchRequestId { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}
