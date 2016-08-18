using System.Linq;
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

        public async Task<bool> ConnectToApplication(string applicationId)
        {
            var startedApplication = Client.ChromecastStatus?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
            if (startedApplication == null) return false;
            if (!string.IsNullOrWhiteSpace(Client.CurrentApplicationSessionId)) return false;
            Client.CurrentApplicationSessionId = startedApplication.SessionId;
            Client.CurrentApplicationTransportId = startedApplication.TransportId;
            await Write(MessageFactory.ConnectWithDestination(startedApplication.TransportId), false);
            Client.RunningApplication = startedApplication;
            return true;
        }


        public async Task GetMediaStatus()
        {
            await Write(MessageFactory.MediaStatus(Client.CurrentApplicationTransportId), false);
        }

        public async Task LaunchApplication(string applicationId, bool joinExisting = true)
        {
            Client.ChromecastApplicationId = applicationId;
            if (joinExisting && await ConnectToApplication(applicationId))
            {
                await GetMediaStatus();
                return;
            }
            await Write(MessageFactory.Launch(applicationId), false);
        }

        
        public async Task StopApplication()
        {
            await Write(MessageFactory.Stop(Client.CurrentApplicationSessionId), false);
        }


        public async void OpenConnection()
        {
            await Write(MessageFactory.Connect());
        }
    }
}
