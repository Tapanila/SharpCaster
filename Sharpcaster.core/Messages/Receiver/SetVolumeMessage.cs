using Sharpcaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages.Receiver
{
    [DataContract]
    public class SetVolumeMessage : MessageWithId
    {
        [DataMember(Name = "volume")]
        public Volume Volume { get; set; }

    }
}
