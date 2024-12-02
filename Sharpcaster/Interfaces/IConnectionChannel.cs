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
        Task ConnectAsync();
        /// <summary>
        /// Connects
        /// </summary>
        /// <param name="transportId">destination identifier</param>
        Task ConnectAsync(string transportId);
    }
}
