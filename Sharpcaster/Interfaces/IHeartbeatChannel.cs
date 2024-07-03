using System.Net.NetworkInformation;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for the heartbeat channel
    /// </summary>
    interface IHeartbeatChannel : IChromecastChannel
    {
        void StartTimeoutTimer();
        void StopTimeoutTimer();
        /// <summary>
        /// Raised when the status has changed
        /// </summary>
        event EventHandler StatusChanged;
    }
}
