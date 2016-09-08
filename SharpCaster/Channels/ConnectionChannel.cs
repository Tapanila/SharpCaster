using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Channels
{
    public class ConnectionChannel : ChromecastChannel
    {
        public ConnectionChannel(ChromeCastClient client) :
            base(client, MessageFactory.DialConstants.DialConnectionUrn)
        {
        }

        public async void OpenConnection()
        {
            await Write(MessageFactory.Connect());
        }

        public async Task ConnectWithDestination()
        {
            await Write(MessageFactory.ConnectWithDestination(Client.CurrentApplicationTransportId));
        }
    }
}
