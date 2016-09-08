using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Models.Metadata;

namespace SharpCaster.Channels
{
    public class MediaChannel : ChromecastChannel
    {
        public MediaChannel(ChromeCastClient client) 
            :base(client, MessageFactory.DialConstants.DialMediaUrn)
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


        public async Task GetMediaStatus()
        {
            await Write(MessageFactory.MediaStatus(Client.CurrentApplicationTransportId));
        }

        public async Task Seek(double seconds)
        {
            await Write(MessageFactory.Seek(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId, seconds));
        }

        public async Task Pause()
        {
            await Write(MessageFactory.Pause(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId));
        }

        public async Task Play()
        {
            await Write(MessageFactory.Play(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId));
        }

        public async Task Stop()
        {
            await Write(MessageFactory.StopMedia(Client.CurrentMediaSessionId));
        }

        public async Task Next()
        {
            await Write(MessageFactory.Next(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId));
        }

        public async Task Previous()
        {
            await Write(MessageFactory.Previous(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId));
        }

        public async Task LoadMedia(
            string mediaUrl,
            string contentType = "application/vnd.ms-sstr+xml",
            IMetadata metadata = null,
            string streamType = "BUFFERED",
            double duration = 0D,
            object customData = null,
            Track[] tracks = null,
            int[] activeTrackIds = null,
            bool autoPlay = true,
            double currentTime = 0.0)
        {
            var mediaObject = new MediaData(mediaUrl, contentType, metadata, streamType, duration, customData, tracks);
            var req = new LoadRequest(Client.CurrentApplicationSessionId, mediaObject, autoPlay, currentTime, customData, activeTrackIds);

            var reqJson = req.ToJson();
            await Write(MessageFactory.Load(Client.CurrentApplicationTransportId, reqJson));
        }
    }
}
