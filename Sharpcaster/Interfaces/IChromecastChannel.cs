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
        IChromecastClient Client { get; set; }

        /// <summary>
        /// Gets the full namespace
        /// </summary>
        string Namespace { get; }

        ILogger _logger { get; }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        Task OnMessageReceivedAsync(IMessage message);
    }
}
