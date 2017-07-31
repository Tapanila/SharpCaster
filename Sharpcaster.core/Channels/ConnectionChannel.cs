using Sharpcaster.Core.Interfaces;
using Sharpcaster.Core.Messages.Chromecast;
using Sharpcaster.Core.Messages.Connection;
using Sharpcaster.Core.Models.ChromecastStatus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpcaster.Core.Channels
{
    /// <summary>
    /// Connection channel
    /// </summary>
    public class ConnectionChannel : ChromecastChannel, IConnectionChannel
    {
        /// <summary>
        /// Initializes a new instance of ConnectionChannel class
        /// </summary>
        public ConnectionChannel() : base("tp.connection")
        {
        }

        /// <summary>
        /// Connects 
        /// </summary>
        /// <param name="destinationId">destination identifier</param>
        public async Task ConnectAsync(string destinationId)
        {
            await SendAsync(new ConnectMessage(), destinationId);
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public async override Task OnMessageReceivedAsync(IMessage message)
        {
            if (message is CloseMessage)
            {
                await Client.DisconnectAsync();
            }
            await base.OnMessageReceivedAsync(message);
        }
    }
}
