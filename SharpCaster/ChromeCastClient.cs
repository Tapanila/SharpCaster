using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Interfaces;
using SharpCaster.Services;
using SharpCaster.MediaControllers;

namespace SharpCaster
{
    public class ChromeCastClient
    {
        public Volume Volume { get; private set; }
        public ChromecastStatus ChromecastStatus { get; set; }
        public MediaStatus MediaStatus { get; set; }
        public IChromecastSocketService ChromecastSocketService { get; set; }

        public IMediaController MediaController { get; set; }

        internal string CurrentApplicationTransportId
        {
            get { return _currentApplicationTransportId; }
        }

        internal string CurrentApplicationSessionId
        {
            get { return _currentApplicationSessionId; }
        }

        Dictionary<string, IMediaController> RegisteredMediaControllers { get; set; }

        private ChromecastChannel _connectionChannel;
        private ChromecastChannel _heartbeatChannel;
        private ChromecastChannel _receiverChannel;
        private const string ChromecastPort = "8009";
        private string _chromecastApplicationId;
        private string _currentApplicationSessionId = "";
        private string _currentApplicationTransportId = "";
        private bool _connected;

        public event EventHandler Connected;
        public event EventHandler<ChromecastApplication> ApplicationStarted;

        //TODO move MediaStatusChanged event to IMediaController?
        public event EventHandler<MediaStatus> MediaStatusChanged;

        public event EventHandler<ChromecastStatus> ChromecastStatusChanged;

        //TODO According to https://developers.google.com/cast/docs/reference/messages#SetVolume
        // There is a difference between device volume and stream volume.
        // Since this property is part of ChromeCastClient it should be used
        // only for device volume!
        public event EventHandler<Volume> VolumeChanged;
        public List<ChromecastChannel> Channels;

        public ChromeCastClient()
        {
            ChromecastSocketService = new ChromecastSocketService();
            Channels = new List<ChromecastChannel>();
            _connectionChannel = CreateChannel(MessageFactory.DialConstants.DialConnectionUrn);
            _heartbeatChannel = CreateChannel(MessageFactory.DialConstants.DialHeartbeatUrn);
            _receiverChannel = CreateChannel(MessageFactory.DialConstants.DialReceiverUrn);

            _receiverChannel.MessageReceived += ReceiverChannel_MessageReceived;
            _heartbeatChannel.MessageReceived += HeartbeatChannel_MessageReceived;

            RegisterMediaControllers();
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
            await ChromecastSocketService.Write(MessageFactory.Volume(level).ToProto());
        }

        public async Task SetMute(bool muted)
        {
            await ChromecastSocketService.Write(MessageFactory.Volume(muted).ToProto());
        }

        public async Task IncreaseVolume()
        {
            await SetVolume(Volume.level + 0.05f);
        }

        public async Task DecreaseVolume()
        {
            await SetVolume(Volume.level - 0.05f);
        }

        private async void HeartbeatChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            if (_connected || e.Message.GetJsonType() != "PONG") return;
            //Wait 100 milliseconds before sending GET_STATUS because chromecast was sending CLOSE back without a wait
            await Task.Delay(100);
            GetChromecastStatus();
            //Wait 100 milliseconds to make sure that the status of Chromecast device is received before notifying we have connected to it
            await Task.Delay(100);
            _connected = true;
            Connected?.Invoke(this, EventArgs.Empty);

        }

        internal void UpdateVolume(Volume volume)
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
            await ChromecastSocketService.Write(MessageFactory.ConnectWithDestination(startedApplication.TransportId).ToProto());
            ApplicationStarted?.Invoke(this, startedApplication);
            return true;
        }



        public async void ConnectChromecast(Uri uri)
        {
            await ChromecastSocketService.Initialize(uri.Host, ChromecastPort, _connectionChannel, _heartbeatChannel, ReadPacket);
        }

        private void RegisterMediaControllers()
        {
            var mediaControllersToRegister = new List<IMediaController>
            {
                new DefaultMediaController(this),
                new PlexMediaController(this),
                new DSAudioMediaController(this)
            };

            RegisteredMediaControllers = mediaControllersToRegister
                .ToDictionary(mediaController => mediaController.DefaultAppId);
        }

        // TODO call this when the app on the chromecast switches
        private void LoadMediaController()
        {
            //todo hot to get the active application ID? to make it dynamic
            var currentApplicationId = new DefaultMediaController(this).DefaultAppId;

            if (RegisteredMediaControllers.Keys.Contains(currentApplicationId))
            {
                MediaController = RegisteredMediaControllers[currentApplicationId];
            }
        }


        public async Task LaunchApplication(string applicationId, bool joinExisting = true)
        {
            _chromecastApplicationId = applicationId;
            if (joinExisting && await ConnectToApplication(applicationId))
            {
                LoadMediaController();

                if (MediaController.SupportsCommand(SupportedCommand.GetMediaStatus))
                {
                    await MediaController.GetMediaStatus();
                }

                return;
            }
            await ChromecastSocketService.Write(MessageFactory.Launch(applicationId).ToProto());
        }

        public async Task StopApplication()
        {
            await ChromecastSocketService.Write(MessageFactory.Stop(_currentApplicationSessionId).ToProto());
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




        private ChromecastChannel CreateChannel(string channelNamespace)
        {
            var channel = new ChromecastChannel(ChromecastSocketService, channelNamespace);
            Channels.Add(channel);
            return channel;
        }

        //TODO remove this and move MediaStatusChanged event to IMediaController?
        internal void InvokeMediaStatusChanged(MediaStatus mediaStatus)
        {
            MediaStatusChanged?.Invoke(this, mediaStatus);
        }

    }
}
