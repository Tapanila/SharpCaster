using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpcaster.Core.Interfaces
{
    /// <summary>
    /// Interface for messages with request identifier
    /// </summary>
    public interface IMessageWithId : IMessage
    {
        /// <summary>
        /// Gets the request identifier
        /// </summary>
        int RequestId { get; }
    }
}
