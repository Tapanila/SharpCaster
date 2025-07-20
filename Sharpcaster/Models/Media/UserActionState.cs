using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Represents user action state for media
    /// </summary>
    public class UserActionState
    {
        /// <summary>
        /// The user action
        /// </summary>
        [JsonPropertyName("userAction")]
        public UserAction UserAction { get; set; }

        /// <summary>
        /// Optional app specific data
        /// </summary>
        [JsonPropertyName("customData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? CustomData { get; set; }
    }
}