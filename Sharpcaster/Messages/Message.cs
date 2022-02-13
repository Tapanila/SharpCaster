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
            //TODO: Why is type like this
            var type = GetType().Name;
            var firstCharacter = true;
            Type = string.Concat(type.Substring(0, type.LastIndexOf(nameof(Message))).Select(c =>
            {
                if (firstCharacter)
                {
                    firstCharacter = false;
                }
                else
                {
                    if (char.IsUpper(c))
                    {
                        return $"_{c}";
                    }
                }

                return char.ToUpper(c).ToString();
            }));
        }

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
