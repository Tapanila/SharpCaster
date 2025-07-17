using Sharpcaster.Interfaces;
using System;
using System.Text.Json.Serialization;
using System.Threading;

namespace Sharpcaster.Messages
{
    /// <summary>
    /// Message with request identifier
    /// </summary>
    public class MessageWithId : Message, IMessageWithId
    {
        private static int _id = new Random().Next();

        /// <summary>
        /// Gets a value indicating whether the message has a request identifier
        /// </summary>
        [JsonIgnore]
        public bool HasRequestId
        {
            get { return _requestId != null && _requestId != 0; }
        }

        private int? _requestId;
        /// <summary>
        /// Gets or sets the request identifier
        /// </summary>
        [JsonPropertyName("requestId")]
        public int RequestId
        {
            get { return (int)(_requestId ?? (_requestId = Interlocked.Increment(ref _id))); }
            set { _requestId = value; }
        }
    }
}
