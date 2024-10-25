using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Messages.Media
{
    [DataContract]
    [ReceptionMessage]
    public class ErrorMessage : Message
    {
        [DataMember(Name = "detailedErrorCode")]
        public int DetailedErrorCode { get; set; }
        [DataMember(Name = "itemId")]
        public int ItemId { get; set; }
    }
}
