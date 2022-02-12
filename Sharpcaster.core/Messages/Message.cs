using Sharpcaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sharpcaster.Core.Messages
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
            Type = String.Concat(type.Substring(0, type.LastIndexOf(nameof(Message))).Select(c =>
            {
                if (firstCharacter)
                {
                    firstCharacter = false;
                }
                else
                {
                    if (Char.IsUpper(c))
                    {
                        return $"_{c}";
                    }
                }

                return Char.ToUpper(c).ToString();
            }));
        }

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
