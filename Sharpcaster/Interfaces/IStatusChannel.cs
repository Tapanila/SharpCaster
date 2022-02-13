using System;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for a status channel
    /// </summary>
    /// <typeparam name="TStatus">status type</typeparam>
    public interface IStatusChannel<TStatus> : IChromecastChannel
    {
        /// <summary>
        /// Raised when the status has changed
        /// </summary>
        event EventHandler StatusChanged;

        /// <summary>
        /// Gets the status
        /// </summary>
        TStatus Status { get; }
    }
}
