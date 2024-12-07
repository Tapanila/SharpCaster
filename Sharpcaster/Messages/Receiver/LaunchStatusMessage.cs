using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sharpcaster.Messages.Receiver
{
    public class LaunchStatusMessage : MessageWithId
    {
        [JsonPropertyName("launchRequestId")]
        public int LaunchRequestId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
