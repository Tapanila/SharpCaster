using Microsoft.Extensions.Logging;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Connection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Connection channel, Responsible for opening connection to Chromecast and receiving Closed message
    /// </summary>
    public class ConnectionChannel : ChromecastChannel
    {
        /// <summary>
        /// Initializes a new instance of ConnectionChannel class
        /// </summary>
        public ConnectionChannel(ILogger? log = null) : base("tp.connection", log)
        {
        }

        /// <summary>
        /// Connects to chromecast
        /// </summary>
        public async Task ConnectAsync()
        {
            await ConnectAsync("receiver-0").ConfigureAwait(false);
        }

        /// <summary>
        /// Connects to running chromecast application
        /// </summary>
        public async Task ConnectAsync(string transportId)
        {
            var connectMessage = new ConnectMessage();
            await SendAsync(JsonSerializer.Serialize(connectMessage, SharpcasteSerializationContext.Default.ConnectMessage), transportId).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="messagePayload">message payload to process</param>
        /// <param name="type">message type</param>
        public async override Task OnMessageReceivedAsync(string messagePayload, string type)
        {
            if (type == "CLOSE")
            {
                // In order to avoid usage deadlocks we need to spawn a new Task here!?
                _ = Task.Run(async () => await Client.DisconnectAsync().ConfigureAwait(false));
            }
            await base.OnMessageReceivedAsync(messagePayload, type).ConfigureAwait(false);
        }
    }
}
