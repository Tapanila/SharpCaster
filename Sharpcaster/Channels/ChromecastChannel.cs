using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Channel base class
    /// </summary>
    public abstract class ChromecastChannel : IChromecastChannel
    {
        private const string BASE_NAMESPACE = "urn:x-cast:com.google.cast";
        public ILogger? Logger { get; }

        /// <summary>
        /// Initialization
        /// </summary>
        protected ChromecastChannel()
        {
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="ns">namespace</param>
        /// <param name="useBaseNamespace">When true add urn:x-cast:com.google.cast to beginning of namespace</param>
        protected ChromecastChannel(string ns, ILogger? logger, bool useBaseNamespace = true)
        {
            Logger = logger;
            if (useBaseNamespace)
            {
                Namespace = $"{BASE_NAMESPACE}.{ns}";
            }
            else
            {
                Namespace = ns;
            }
        }

        /// <summary>
        /// Gets or sets the sender
        /// </summary>
        public virtual ChromecastClient Client { get; set; }

        /// <summary>
        /// Gets the full namespace
        /// </summary>
        public string Namespace { get; protected set; }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message">message to send</param>
        /// <param name="destinationId">destination identifier</param>
        protected async Task<string> SendAsync(int messageRequestId, string messagePayload, string destinationId = DefaultIdentifiers.DESTINATION_ID)
        {
            return await Client.SendAsync(Logger, Namespace, messageRequestId, messagePayload, destinationId);
        }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message">message to send</param>
        /// <param name="destinationId">destination identifier</param>
        protected async Task SendAsync(string messagePayload, string destinationId = DefaultIdentifiers.DESTINATION_ID)
        {
            await Client.SendAsync(Logger, Namespace, messagePayload, destinationId);
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public virtual Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            return Task.CompletedTask;
        }
    }
}
