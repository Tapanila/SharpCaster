using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;

namespace SharpCaster.Channels
{
    public class ReceiverChannel : ChromecastChannel 
    {
        public ReceiverChannel(ChromeCastClient client) :
            base(client, "urn:x-cast:com.google.cast.receiver")
        {
            MessageReceived += ReceiverChannel_MessageReceived;
        }

        private async void ReceiverChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<ChromecastStatusResponse>(json);
            if (response.ChromecastStatus == null) return;
            Client.ChromecastStatus = response.ChromecastStatus;
            Client.Volume = response.ChromecastStatus.Volume;
            await ConnectToApplication(Client.ChromecastApplicationId);
        }

        public async Task LaunchApplication(string applicationId, bool joinExisting = true)
        {
            Client.ChromecastApplicationId = applicationId;
            if (joinExisting && await ConnectToApplication(applicationId))
            {
                await Client.MediaChannel.GetMediaStatus();
                return;
            }
            await Write(MessageFactory.Launch(applicationId));
        }

        public async Task<bool> ConnectToApplication(string applicationId)
        {
            var startedApplication = Client.ChromecastStatus?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
            if (startedApplication == null) return false;
            if (!string.IsNullOrWhiteSpace(Client.CurrentApplicationSessionId)) return false;
            Client.CurrentApplicationSessionId = startedApplication.SessionId;
            Client.CurrentApplicationTransportId = startedApplication.TransportId;
            await Client.ConnectionChannel.ConnectWithDestination();
            Client.RunningApplication = startedApplication;
            return true;
        }


        public async void GetChromecastStatus()
        {
            await Write(MessageFactory.Status());
        }


        public async Task SetMute(bool muted)
        {
            await Write(MessageFactory.Volume(muted));
        }

        public async Task IncreaseVolume(double amount = 0.05)
        {
            await SetVolume(Client.Volume.level + amount);
        }

        public async Task DecreaseVolume(double amount = 0.05)
        {
            await SetVolume(Client.Volume.level - amount);
        }

        public async Task SetVolume(double level)
        {
            if (level < 0 || level > 1.0)
            {
                throw new ArgumentException("level must be between 0.0f and 1.0f", nameof(level));
            }
            await Write(MessageFactory.Volume(level));
        }

        public async Task StopApplication()
        {
            await Write(MessageFactory.StopApplication(Client.CurrentApplicationSessionId));
        }


    }
}
