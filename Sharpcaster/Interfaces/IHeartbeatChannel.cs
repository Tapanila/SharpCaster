using System;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for the heartbeat channel
    /// </summary>
    public interface IHeartbeatChannel : IChromecastChannel
    {
        void StartTimeoutTimer();
        void StopTimeoutTimer();
        /// <summary>
        /// Raised when the status has changed
        /// </summary>
        event EventHandler StatusChanged;
    }
}
