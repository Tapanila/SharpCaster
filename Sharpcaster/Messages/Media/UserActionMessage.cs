using System.Text.Json.Serialization;
using Sharpcaster.Models.Media;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// User action message
    /// </summary>
    public class UserActionMessage : Message
    {
        /// <summary>
        /// Gets or sets the user action
        /// </summary>
        [JsonPropertyName("userAction")]
        public UserAction UserAction { get; set; }
    }
}