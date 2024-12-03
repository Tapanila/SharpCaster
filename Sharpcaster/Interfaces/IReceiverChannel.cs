using Sharpcaster.Messages.Receiver;
using Sharpcaster.Models.ChromecastStatus;
using System;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for the receiver channel
    /// </summary>
    public interface IReceiverChannel : IStatusChannel<ChromecastStatus>, IChromecastChannel
    {
        /// <summary>
        /// Launches an application
        /// </summary>
        /// <param name="applicationId">application identifier</param>
        /// <returns>receiver status</returns>
        Task<ChromecastStatus> LaunchApplicationAsync(string applicationId);
        Task<ChromecastStatus> GetChromecastStatusAsync();
        Task<ChromecastStatus> StopApplication();
        Task<ChromecastStatus> SetVolume(double level);
        Task<ChromecastStatus> SetMute(bool muted);

        event EventHandler<LaunchStatusMessage> LaunchStatusChanged;
    }
}
