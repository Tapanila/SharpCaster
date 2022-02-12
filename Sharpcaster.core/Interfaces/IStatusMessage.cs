using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpcaster.Core.Interfaces
{
    /// <summary>
    /// Interface for status messages
    /// </summary>
    /// <typeparam name="TStatus">status type</typeparam>
    public interface IStatusMessage<TStatus> : IMessageWithId
    {
        /// <summary>
        /// Gets the status
        /// </summary>
        TStatus Status { get; }
    }
}
