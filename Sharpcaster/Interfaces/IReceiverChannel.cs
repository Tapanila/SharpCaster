using Sharpcaster.Models.ChromecastStatus;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for the receiver channel
    /// </summary>
    public interface IReceiverChannel : IChromecastChannel
    {
        /// <summary>
        /// Launches an application
        /// </summary>
        /// <param name="applicationId">application identifier</param>
        /// <returns>receiver status</returns>
        Task<ChromecastStatus> LaunchApplicationAsync(string applicationId);

        Task<ChromecastStatus> GetChromecastStatusAsync();
        Task<ChromecastStatus> StopApplication(string sessionId);
        Task<ChromecastStatus> SetVolume(double level);
        Task<ChromecastStatus> SetMute(bool muted);
    }
}
