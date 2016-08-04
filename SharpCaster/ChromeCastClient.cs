using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Channels;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Interfaces;
using SharpCaster.Services;

namespace SharpCaster
{
    public class ChromeCastClient
    {
        public Volume Volume { get; private set; }
        public ChromecastStatus ChromecastStatus { get; set; }
        public MediaStatus MediaStatus { get; set; }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (_connected != value) ConnectedChanged?.Invoke(this, EventArgs.Empty);
                _connected = value;
            }
        }

        private bool _connected;

        public IChromecastSocketService ChromecastSocketService {get; set; }

        private IChromecastChannel _connectionChannel;
        private IChromecastChannel _mediaChannel;
        private HeartbeatChannel _heartbeatChannel;
        private IChromecastChannel _receiverChannel;
        private const string ChromecastPort = "8009";
        private string _chromecastApplicationId;
        private string _currentApplicationSessionId = "";
        private string _currentApplicationTransportId = "";
        private long _currentMediaSessionId;
        
        public event EventHandler ConnectedChanged;
        public event EventHandler<ChromecastApplication> ApplicationStarted;
        public event EventHandler<MediaStatus> MediaStatusChanged;
        public event EventHandler<ChromecastStatus> ChromecastStatusChanged;
        public event EventHandler<Volume> VolumeChanged;
        public List<IChromecastChannel> Channels;

        public ChromeCastClient()
        {
            ChromecastSocketService = new ChromecastSocketService();
            Channels = new List<IChromecastChannel>();

            _connectionChannel = CreateChannel(MessageFactory.DialConstants.DialConnectionUrn);
            _heartbeatChannel = new HeartbeatChannel(this);
            Channels.Add(_heartbeatChannel);
            _receiverChannel = CreateChannel(MessageFactory.DialConstants.DialReceiverUrn);
            _mediaChannel = CreateChannel(MessageFactory.DialConstants.DialMediaUrn);

            _mediaChannel.MessageReceived += MediaChannel_MessageReceived;
            _receiverChannel.MessageReceived += ReceiverChannel_MessageReceived;
        }

        private IChromecastChannel CreateChannel(string ns)
        {
            var channel = new DefaultChannel(this,ns);
            Channels.Add(channel);
            return channel;
        }

        public async void GetChromecastStatus()
        {
            await _receiverChannel.Write(MessageFactory.Status());
        }

        public async Task SetVolume(float level)
        {
            if (level < 0 || level > 1.0f)
            {
                throw new ArgumentException("level must be between 0.0f and 1.0f", nameof(level));
            }
            await Write(MessageFactory.Volume(level).ToProto());
        }

        public async Task SetMute(bool muted)
        {
            await Write(MessageFactory.Volume(muted).ToProto());
        }

        public async Task IncreaseVolume()
        {
            await SetVolume(Volume.level + 0.05f);
        }

        public async Task DecreaseVolume()
        {
            await SetVolume(Volume.level - 0.05f);
        }

        public async Task Seek(double seconds)
        {
            await Write(MessageFactory.Seek(_currentApplicationTransportId, _currentMediaSessionId, seconds).ToProto());
        }

        public async Task Pause()
        {
            await Write(MessageFactory.Pause(_currentApplicationTransportId, _currentMediaSessionId).ToProto());
        }

        public async Task Play()
        {
            await Write(MessageFactory.Play(_currentApplicationTransportId, _currentMediaSessionId).ToProto());
        }

        public async Task LoadMedia(string mediaUrl, object customData = null)
        {
            var mediaObject = new MediaData(mediaUrl, "application/vnd.ms-sstr+xml", null, "BUFFERED", 0D, customData);
            var req = new LoadRequest(_currentApplicationSessionId, mediaObject, true, 0.0, customData);

            var reqJson = req.ToJson();
            await _mediaChannel.Write(MessageFactory.Load(_currentApplicationTransportId, reqJson));
        }

        private void MediaChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<MediaStatusResponse>(json);
            if (response.status?.Count < 1) return;
            MediaStatus = response.status.First();
            MediaStatusChanged?.Invoke(this, MediaStatus);
            if (MediaStatus.volume != null) UpdateVolume(MediaStatus.volume);
            _currentMediaSessionId = MediaStatus.mediaSessionId;
        }

        private void UpdateVolume(Volume volume)
        {
            if (Volume != null &&
                !(Math.Abs(Volume.level - volume.level) > 0.01f) &&
                Volume.muted == volume.muted) return;
            Volume = volume;
            VolumeChanged?.Invoke(this, Volume);
        }

        private async void ReceiverChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<ChromecastStatusResponse>(json);
            if (response.ChromecastStatus == null) return;
            ChromecastStatus = response.ChromecastStatus;
            ChromecastStatusChanged?.Invoke(this, ChromecastStatus);
            UpdateVolume(ChromecastStatus.Volume);
            await ConnectToApplication(_chromecastApplicationId);
        }

        private async Task<bool> ConnectToApplication(string applicationId)
        {
            var startedApplication = ChromecastStatus?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
            if (startedApplication == null) return false;
            if (!string.IsNullOrWhiteSpace(_currentApplicationSessionId)) return false;
            _currentApplicationSessionId = startedApplication.SessionId;
            _currentApplicationTransportId = startedApplication.TransportId;
            await Write(MessageFactory.ConnectWithDestination(startedApplication.TransportId).ToProto());
            ApplicationStarted?.Invoke(this, startedApplication);
            return true;
        }



        public async void ConnectChromecast(Uri uri)
        {
            await ChromecastSocketService.Initialize(uri.Host, ChromecastPort, _connectionChannel, _heartbeatChannel, ReadPacket);
        }

        public async Task GetMediaStatus()
        {
            await Write(MessageFactory.MediaStatus(_currentApplicationTransportId).ToProto());
        }

        public async Task LaunchApplication(string applicationId, bool joinExisting = true)
        {
            _chromecastApplicationId = applicationId;
            if (joinExisting && await ConnectToApplication(applicationId))
            {
                await GetMediaStatus();
                return;
            }
            await Write(MessageFactory.Launch(applicationId).ToProto());
        }

        public async Task StopApplication()
        {
            await Write(MessageFactory.Stop(_currentApplicationSessionId).ToProto());
        }

        private void ReadPacket(Stream stream, bool parsed)
        {
            try
            {
                IEnumerable<byte> entireMessage;
                if (parsed)
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    entireMessage = buffer;
                }
                else
                {
                    entireMessage = stream.ParseData();
                }
                
                var entireMessageArray = entireMessage.ToArray();
                var castMessage = entireMessageArray.ToCastMessage();
                if (castMessage == null) return;
                Debug.WriteLine("Received: " + castMessage.GetJsonType());
                if (string.IsNullOrEmpty(castMessage.Namespace)) return;
                ReceivedMessage(castMessage);
            }
            catch (Exception ex)
            {
                // Log these bytes
                Debug.WriteLine(ex);
            }

        }

        private void ReceivedMessage(CastMessage castMessage)
        {
            foreach (var channel in Channels.Where(i => i.Namespace == castMessage.Namespace))
            {
                channel.OnMessageReceived(new ChromecastSSLClientDataReceivedArgs(castMessage));
            }
        }

       
        internal async Task Write(byte[] bytes)
        {
            await ChromecastSocketService.Write(bytes);
        }


    }
}
