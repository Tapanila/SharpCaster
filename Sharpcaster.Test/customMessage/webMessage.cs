using System.Text.Json.Serialization;
using Sharpcaster.Messages;

namespace Sharpcaster.Test.customChannel
{
    public class WebMessage : MessageWithSession
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}