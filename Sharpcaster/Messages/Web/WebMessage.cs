using System.Text.Json.Serialization;
using Sharpcaster.Messages;

namespace Sharpcaster.Messages.Web
{
    public class WebMessage : MessageWithSession
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}