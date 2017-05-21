using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Channels
{
    public class ConnectionChannel : ChromecastChannel
    {
        public ConnectionChannel(ChromeCastClient client) :
            base(client, "urn:x-cast:com.google.cast.tp.connection")
        {
        }

        public async Task OpenConnection()
        {
            await Write(MessageFactory.Connect());
        }

        public async Task ConnectWithDestination()
        {
            await Write(MessageFactory.ConnectWithDestination(Client.CurrentApplicationTransportId));
        }
    }
}
