using System.Globalization;
using System.Text.Json.Serialization;
using Sharpcaster.Interfaces;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message base class
    /// </summary>
    public abstract class Message : IMessage
    {
        /// <summary>
        /// Initialization
        /// </summary>
        protected Message()
        {
            var type = GetType().Name;
            // Get the type name without the "Message" suffix
            type = type.Replace("Message", "");
            var firstCharacter = true;
            var result = "";
            // Convert the type name to uppercase with underscores
            // example: "ReceiverStatusMessage" -> "RECEIVER_STATUS"
            for (int i = 0; i < type.Length; i++)
            {
                var c = type[i];
                if (firstCharacter)
                {
                    firstCharacter = false;
                    result += char.ToUpper(c, CultureInfo.InvariantCulture).ToString();
                }
                else if (char.IsUpper(c))
                {
                    result += $"_{c}";
                }
                else
                {
                    result += char.ToUpper(c, CultureInfo.InvariantCulture).ToString();
                }
            }
            Type = result;
        }

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}