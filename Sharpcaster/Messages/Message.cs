using Sharpcaster.Interfaces;
using System.Linq;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message base class
    /// </summary>
    [DataContract]
    public abstract class Message : IMessage
    {
        /// <summary>
        /// Initialization
        /// </summary>
        public Message()
        {
            var type = GetType().Name;
            //Get the type name without the "Message" suffix
            type = type.Replace("Message", "");
            var firstCharacter = true;
            var result = "";
            //Convert the type name to uppercase with underscores
            //example: "ReceiverStatusMessage" -> "RECEIVER_STATUS"
            for (int i = 0; i < type.Length; i++)
            {
                var c = type[i];
                if (firstCharacter)
                {
                    firstCharacter = false;
                    result += char.ToUpper(c).ToString();
                }
                else if (char.IsUpper(c))
                {
                    result += $"_{c}";
                }
                else
                {
                    result += char.ToUpper(c).ToString();
                }
            }
            Type = result;
        }

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
