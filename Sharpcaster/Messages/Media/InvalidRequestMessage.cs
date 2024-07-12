using System;
using System.Runtime.Serialization;

namespace Sharpcaster.Messages.Media
{
    /// <summary>
    /// Invalid request message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    public class InvalidRequestMessage : MessageWithId
    {
        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        [DataMember(Name = "reason")]
        public string Reason { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            throw new InvalidOperationException(Reason);
        }
    }
}
