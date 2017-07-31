using Sharpcaster.Core.Models.ChromecastStatus;
using System.Threading.Tasks;

namespace Sharpcaster.Core.Interfaces
{
    /// <summary>
    /// Interface for the connection channel
    /// </summary>
    public interface IConnectionChannel : IChromecastChannel
    {
        /// <summary>
        /// Connects 
        /// </summary>
        /// <param name="destinationId">destination identifier</param>
        Task ConnectAsync(string destinationId = "receiver-0");
    }
}
