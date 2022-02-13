using Sharpcaster.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message with request identifier
    /// </summary>
    [DataContract]
    public class MessageWithId : Message, IMessageWithId
    {
        private static int _id = new Random().Next();

        /// <summary>
        /// Gets a value indicating whether the message has a request identifier
        /// </summary>
        public bool HasRequestId
        {
            get { return _requestId != null; }
        }

        private int? _requestId;
        /// <summary>
        /// Gets or sets the request identifier
        /// </summary>
        [DataMember(Name = "requestId")]
        public int RequestId
        {
            get { return (int)(_requestId ?? (_requestId = Interlocked.Increment(ref _id))); }
            set { _requestId = value; }
        }
    }
}
