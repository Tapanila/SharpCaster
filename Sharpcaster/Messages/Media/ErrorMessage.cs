using System.Text.Json.Serialization;

namespace Sharpcaster.Messages.Media
{
    [ReceptionMessage]
    public class ErrorMessage : Message
    {
        [JsonPropertyName("detailedErrorCode")]
        public int DetailedErrorCode { get; set; }
        [JsonPropertyName("itemId")]
        public int ItemId { get; set; }
    }
}
