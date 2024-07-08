using Microsoft.Extensions.Logging;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Connection;
using System.Threading.Tasks;

namespace Sharpcaster.Channels
{
    /// <summary>
    /// Connection channel, Responsible for opening connection to Chromecast and receiving Closed message
    /// </summary>
    public class ConnectionChannel : ChromecastChannel, IConnectionChannel
    {
        /// <summary>
        /// Initializes a new instance of ConnectionChannel class
        /// </summary>
        public ConnectionChannel(ILogger<ConnectionChannel> log = null) : base("tp.connection", log)
        {
        }

        /// <summary>
        /// Connects to chromecast
        /// </summary>
        public async Task ConnectAsync()
        {
            await SendAsync(new ConnectMessage());
        }

        /// <summary>
        /// Connects to running chromecast application 
        /// </summary>
        public async Task ConnectAsync(string transportId)
        {
            await SendAsync(new ConnectMessage(), transportId);
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public async override Task OnMessageReceivedAsync(IMessage message)
        {
            if (message is CloseMessage) {
                // In order to avoid usage deadlocks we need to spawn a new Task here!?
                _ = Task.Run(async () => {
                    await Client.DisconnectAsync();
                });
            }
            await base.OnMessageReceivedAsync(message);
        }
    }
}
