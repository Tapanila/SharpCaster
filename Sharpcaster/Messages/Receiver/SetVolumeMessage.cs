using Sharpcaster.Messages.Media;
using Sharpcaster.Models;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Receiver
{
    // https://developers.google.com/cast/docs/reference/web_receiver/cast.framework.messages.VolumeRequestData
    public class SetVolumeMessage : MediaSessionMessage
    {
        [JsonPropertyName("volume")]
        public Volume Volume { get; set; }
    }
}
