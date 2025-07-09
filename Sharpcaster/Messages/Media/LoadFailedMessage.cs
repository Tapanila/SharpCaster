using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Load failed message
    /// </summary>
    [ReceptionMessage]
    public class LoadFailedMessage : MessageWithId
    {
        [JsonPropertyName("itemId")]
        public int ItemId { get; set; }
    }
}
