using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using System;
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
        /// <param name="logger">logger instance</param>
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
        public virtual ChromecastClient Client { get; set; } = null!;

        /// <summary>
        /// Gets the full namespace
        /// </summary>
        public string Namespace { get; protected set; } = string.Empty;

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="messageRequestId">message request identifier</param>
        /// <param name="messagePayload">message payload to send</param>
        /// <param name="destinationId">destination identifier</param>
        protected async Task<string> SendAsync(int messageRequestId, string messagePayload, string destinationId = DefaultIdentifiers.DESTINATION_ID)
        {
            return await Client.SendAsync(Logger, Namespace, messageRequestId, messagePayload, destinationId).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="messagePayload">message payload to send</param>
        /// <param name="destinationId">destination identifier</param>
        protected async Task SendAsync(string messagePayload, string destinationId = DefaultIdentifiers.DESTINATION_ID)
        {
            await Client.SendAsync(Logger, Namespace, messagePayload, destinationId).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="messagePayload">message payload to process</param>
        /// <param name="type">message type</param>
        public virtual void OnMessageReceived(string messagePayload, string type)
        {
        }

        /// <summary>
        /// Safely invokes an event handler asynchronously to prevent blocking the receive loop
        /// </summary>
        /// <typeparam name="T">Event argument type</typeparam>
        /// <param name="eventHandler">Event handler to invoke</param>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        protected static void SafeInvokeEvent<T>(EventHandler<T>? eventHandler, object sender, T args)
        {
            if (eventHandler != null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        eventHandler.Invoke(sender, args);
                    }
                    catch
                    {
                        // Swallow exceptions from event handlers to prevent them from crashing the receive loop
                    }
                });
            }
        }

        /// <summary>
        /// Safely invokes an event handler asynchronously to prevent blocking the receive loop
        /// </summary>
        /// <param name="eventHandler">Event handler to invoke</param>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        protected static void SafeInvokeEvent(EventHandler? eventHandler, object sender, EventArgs args)
        {
            if (eventHandler != null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        eventHandler.Invoke(sender, args);
                    }
                    catch
                    {
                        // Swallow exceptions from event handlers to prevent them from crashing the receive loop
                    }
                });
            }
        }
    }
}
