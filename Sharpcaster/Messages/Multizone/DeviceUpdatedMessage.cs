using Sharpcaster.Models.MultiZone;
using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Multizone
{
    /// <summary>
    /// Media status message
    /// </summary>
    [ReceptionMessage]
    public class DeviceUpdatedMessage : MessageWithId
    {
        [JsonPropertyName("device")]
        public Device Device { get; set; }
    }
}
