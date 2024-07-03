using System.Runtime.Serialization;
using Newtonsoft.Json;
using Sharpcaster.Messages;

namespace Sharpcaster.Test.customChannel
{
    [DataContract]
    public class WebMessage : MessageWithId
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}