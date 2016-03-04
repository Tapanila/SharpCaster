using System; 
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster
{
    public class ChromeCastClient
    {
        public Volume Volume { get; private set; }
        public MediaStatus MediaStatus { get; set; }

        private ChromecastChannel _connectionChannel;
        private ChromecastChannel _mediaChannel;
        private ChromecastChannel _heartbeatChannel;
        private List<ChromecastChannel> _channels;
        private const string ChromecastPort = "8009";
        private string _chromecastApplicationId;
        private string _currentApplicationSessionId = "";
        private string _currentApplicationTransportId = "";
        private long _currentMediaSessionId;
        private StreamSocket _socket;
        private bool _connected;

        public event EventHandler Connected;
        public event EventHandler<ChromecastApplication> ApplicationStarted;
        public event EventHandler<MediaStatus> MediaStatusChanged;
        public event EventHandler<Volume> VolumeChanged;

        public ChromeCastClient()
        {
            _channels = new List<ChromecastChannel>();
            _connectionChannel = CreateChannel(MessageFactory.DialConstants.DialConnectionUrn);
            _heartbeatChannel = CreateChannel(MessageFactory.DialConstants.DialHeartbeatUrn);
            var receiverChannel = CreateChannel(MessageFactory.DialConstants.DialReceiverUrn);
            _mediaChannel = CreateChannel(MessageFactory.DialConstants.DialMediaUrn);

            _mediaChannel.MessageReceived += MediaChannel_MessageReceived;
            receiverChannel.MessageReceived += ReceiverChannel_MessageReceived;
            _heartbeatChannel.MessageReceived += HeartbeatChannel_MessageReceived;
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

        private void HeartbeatChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            if (_connected || e.Message.GetJsonType() != "PONG") return;
            _connected = true;
            Connected?.Invoke(this, EventArgs.Empty);
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
            UpdateVolume(response.status.volume);
            var startedApplication = response.status?.applications?.FirstOrDefault(x => x.appId == _chromecastApplicationId);
            if (startedApplication == null) return;
            if (!string.IsNullOrWhiteSpace(_currentApplicationSessionId)) return;
            _currentApplicationSessionId = startedApplication.sessionId;
            _currentApplicationTransportId = startedApplication.transportId;
            await Write(MessageFactory.ConnectWithDestination(startedApplication.transportId).ToProto());
            ApplicationStarted?.Invoke(this, startedApplication);
        }



        public async void ConnectChromecast(Uri uri)
        {
            _socket = new StreamSocket().ConfigureForChromecast();
            await _socket.ConnectAsync(new HostName(uri.Host), ChromecastPort, SocketProtectionLevel.Tls10);

            OpenConnection();
            StartHeartbeat();

            await Task.Run(() =>
            {
                while (true)
                {
                    ReadPacket(_socket.InputStream.AsStreamForRead());
                }
            });
        }

        public async Task LaunchApplication(string applicationId)
        {
            _chromecastApplicationId = applicationId;
            await Write(MessageFactory.Launch(applicationId).ToProto());
        }

        private void ReadPacket(Stream stream)
        {
            try
            {
                var entireMessage = stream.ParseData();
                var entireMessageArray = entireMessage.ToArray();
                var castMessage = entireMessageArray.ToCastMessage();
                if (castMessage == null) return;
                Debug.WriteLine("Received: " + castMessage.GetJsonType());
                if (string.IsNullOrEmpty(castMessage.Namespace)) return;
                ReceivedMessage(castMessage);
            }
            catch (Exception ex)
            {
                // Log these bytes?
                Debug.WriteLine(ex);
            }

        }

        private void ReceivedMessage(CastMessage castMessage)
        {
            foreach (var channel in _channels.Where(i => i.Namespace == castMessage.Namespace))
            {
                channel.OnMessageReceived(new ChromecastSSLClientDataReceivedArgs(castMessage));
            }
        }


        private async void OpenConnection()
        {
            await Write(MessageFactory.Connect().ToProto());
        }

        private ChromecastChannel CreateChannel(string channelNamespace)
        {
            var channel = new ChromecastChannel(this, channelNamespace);
            _channels.Add(channel);
            return channel;
        }

        private void StartHeartbeat()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await _heartbeatChannel.Write(MessageFactory.Ping);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }

        internal async Task Write(byte[] bytes)
        {
            var buffer = CryptographicBuffer.CreateFromByteArray(bytes);
            await _socket.OutputStream.WriteAsync(buffer);
        }


    }
}
