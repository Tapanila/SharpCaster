using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.MediaStatus;

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
            if (Client.MediaStatus.volume != null) Client.Volume = Client.MediaStatus.volume;
            Client.CurrentMediaSessionId = Client.MediaStatus.mediaSessionId;
        }

        public async Task SetVolume(float level)
        {
            if (level < 0 || level > 1.0f)
            {
                throw new ArgumentException("level must be between 0.0f and 1.0f", nameof(level));
            }
            await Write(MessageFactory.Volume(level), false);
        }

        public async Task SetMute(bool muted)
        {
            await Write(MessageFactory.Volume(muted), false);
        }

        public async Task IncreaseVolume(float amount = 0.05f)
        {
            await SetVolume(Client.Volume.level + amount);
        }

        public async Task DecreaseVolume(float amount = 0.05f)
        {
            await SetVolume(Client.Volume.level - amount);
        }

        public async Task Seek(double seconds)
        {
            await Write(MessageFactory.Seek(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId, seconds), false);
        }

        public async Task Pause()
        {
            await Write(MessageFactory.Pause(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId), false);
        }

        public async Task Play()
        {
            await Write(MessageFactory.Play(Client.CurrentApplicationTransportId, Client.CurrentMediaSessionId), false);
        }

        public async Task LoadMedia(
            string mediaUrl,
            string contentType = "application/vnd.ms-sstr+xml",
            Metadata metadata = null,
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
