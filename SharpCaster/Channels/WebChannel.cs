using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.MediaStatus;
using System.Linq;
using SharpCaster.Models.ChromecastRequests;

namespace SharpCaster.Channels
{
    public class WebChannel : ChromecastChannel
    {
        public event EventHandler<string> ScreenIdChanged;

        public WebChannel(ChromeCastClient client) : base(client, "urn:x-cast:com.url.cast")
        {
            MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, ChromecastSSLClientDataReceivedArgs chromecastSSLClientDataReceivedArgs)
        {
            var json = chromecastSSLClientDataReceivedArgs.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<MediaStatusResponse>(json);
            if (response.status == null) return; //TODO: Should probably raise LOAD_FAILED event
            if (response.status.Count == 0) return; //Initializing
            Client.MediaStatus = response.status.First();
            if (Client.MediaStatus.Volume != null) Client.Volume = Client.MediaStatus.Volume;
            Client.CurrentMediaSessionId = Client.MediaStatus.MediaSessionId;
        }

        public async Task LoadUrl(string strUrl)
        {
            var req = new WebRequest(Client.CurrentApplicationSessionId, strUrl);

            var reqJson = req.ToJson();
            await Write(MessageFactory.Load(Client.CurrentApplicationTransportId, reqJson));
        }

        public async Task StopApplication()
        {
            await Write(MessageFactory.StopApplication(Client.CurrentApplicationSessionId));
        }

        public async Task Stop()
        {
            await Write(MessageFactory.StopMedia(Client.CurrentMediaSessionId));
        }

        public async Task GetMediaStatus()
        {
            await Write(MessageFactory.MediaStatus(Client.CurrentApplicationTransportId));
        }        
    }
}
