using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
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
        Task ConnectAsync();
        Task ConnectAsync(string transportId);
    }
}
