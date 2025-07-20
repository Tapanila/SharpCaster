using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for a channel
    /// </summary>
    public interface IChromecastChannel
    {
        /// <summary>
        /// Gets or sets the sender
        /// </summary>
        ChromecastClient Client { get; set; }

        /// <summary>
        /// Gets the full namespace
        /// </summary>
        string Namespace { get; }

        ILogger Logger { get; }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="messagePayload">message to process</param>
        Task OnMessageReceivedAsync(string messagePayload, string type);
    }
}
