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
using SharpCaster.DeviceControllers;

namespace SharpCaster
{
    public class ChromeCastClient
    {
        public Volume Volume { get; private set; }
        public ChromecastStatus ChromecastStatus { get; set; }
        public MediaStatus MediaStatus { get; set; }
        public IChromecastSocketService ChromecastSocketService { get; set; }


        public IConnectionController ConnectionController { get; set; }

        public IReceiverController ReceiverController { get; set; }

        public IHeartbeatController HeartbeatController { get; set; }

        public IMediaController MediaController { get; set; }

        internal string ChromecastApplicationId
        {
            get { return _chromecastApplicationId; }
        }

        internal string CurrentApplicationTransportId
        {
            get { return _currentApplicationTransportId; }
        }

        internal string CurrentApplicationSessionId
        {
            get { return _currentApplicationSessionId; }
        }

        Dictionary<string, IMediaController> RegisteredMediaControllers { get; set; }

        private const string ChromecastPort = "8009";
        private string _chromecastApplicationId;
        private string _currentApplicationSessionId = "";
        private string _currentApplicationTransportId = "";

        private bool _isConnected;

        public event EventHandler Connected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                if (value)
                {
                    Connected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

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


            ConnectionController = new ConnectionController(this);
            ReceiverController = new ReceiverController(this);
            HeartbeatController = new HeartbeatController(this);

            RegisterMediaControllers();
        }





        public async Task IncreaseVolume()
        {
            await ReceiverController.SetVolume(Volume.level + 0.05f);
        }

        public async Task DecreaseVolume()
        {
            await ReceiverController.SetVolume(Volume.level - 0.05f);
        }

        internal void UpdateApplicationConnectionIds(string applicationSessionId, string applicationTransportId)
        {
            _currentApplicationSessionId = applicationSessionId;
            _currentApplicationTransportId = applicationTransportId;
        }

        internal void UpdateStatus(ChromecastStatus status)
        {
            ChromecastStatus = status;
            ChromecastStatusChanged?.Invoke(this, status);
        }

        internal void UpdateVolume(Volume volume)
        {
            if (Volume != null &&
                !(Math.Abs(Volume.level - volume.level) > 0.01f) &&
                Volume.muted == volume.muted) return;
            Volume = volume;
            VolumeChanged?.Invoke(this, Volume);
        }

        public async void ConnectChromecast(Uri uri)
        {
            await ChromecastSocketService.Initialize(uri.Host, ChromecastPort);
            await ConnectionController.OpenConnection();

#pragma warning disable 4014
            /*
             * pragma warning disable since the program should continue
             * while ReadPackets() runs
             */
            ChromecastSocketService.ReadPackets();
#pragma warning restore 4014

            HeartbeatController.StartHeartbeat();
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
            if (joinExisting && await ConnectionController.ConnectToApplication(applicationId))
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

        public ChromecastChannel CreateChannel(string channelNamespace)
        {
            var channel = new ChromecastChannel(ChromecastSocketService, channelNamespace);
            ChromecastSocketService.Channels.Add(channel);
            return channel;
        }

        //TODO remove this and move MediaStatusChanged event to IMediaController?
        internal void InvokeMediaStatusChanged(MediaStatus mediaStatus)
        {
            MediaStatusChanged?.Invoke(this, mediaStatus);
        }

        //TODO 
        internal void InvokeApplicationStarted(ChromecastApplication application)
        {
            ApplicationStarted?.Invoke(this, application);
        }
    }
}
