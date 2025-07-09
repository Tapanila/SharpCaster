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
        event EventHandler<TStatus> StatusChanged;

        /// <summary>
        /// Gets the status
        /// </summary>
        TStatus Status { get; }

        /// <summary>
        /// Clears the status
        /// </summary>
        void ClearStatus();
    }
}
