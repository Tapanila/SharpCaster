using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Api.CastChannel;
using SharpCaster.Channels;
using SharpCaster.Controllers;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Interfaces;
using SharpCaster.Services;

namespace SharpCaster
{
    public class ChromeCastClient
    {
        public Volume Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (_volume != null &&
                    !(Math.Abs(_volume.level - value.level) > 0.01f) &&
                    _volume.muted == value.muted) return;
                _volume = value;
                VolumeChanged?.Invoke(this, _volume);
            }
        }

        private Volume _volume;

        public ChromecastStatus ChromecastStatus
        {
            get
            {
                return _chromecastStatus;
            }
            set
            {
                if (_chromecastStatus == value) return;
                _chromecastStatus = value;
                ChromecastStatusChanged?.Invoke(this, _chromecastStatus);
            }
        }

        private ChromecastStatus _chromecastStatus;

        public MediaStatus MediaStatus
        {
            get
            {
                return _mediaStatus;
            }
            set
            {
                if (_mediaStatus == value) return;
                _mediaStatus = value;
                MediaStatusChanged?.Invoke(this, _mediaStatus);
            }
        }

        private MediaStatus _mediaStatus;

        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (_connected == value) return;
                _connected = value;
                ConnectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _connected;

        public ChromecastApplication RunningApplication
        {
            get
            {
                return _runningApplication;
            }
            set
            {
                if (_runningApplication == value) return;
                _runningApplication = value;
                ApplicationStarted?.Invoke(this, _runningApplication);
            }
        }

        private ChromecastApplication _runningApplication;


        private Dictionary<string, IController> _controllerDictionaryBackingField { get; set; }

        private Dictionary<string, IController> GetControllerDictionary()
        {
            if (_controllerDictionaryBackingField != null) return _controllerDictionaryBackingField;
            var controllers = new List<IController>
            {
                new PlexController(this),
                new YouTubeController(this),
                new SharpCasterDemoController(this)
            };
            _controllerDictionaryBackingField = controllers.ToDictionary(controller => controller.ApplicationId);

            return _controllerDictionaryBackingField;
        }

        public IChromecastSocketService ChromecastSocketService { get; set; }

        public ConnectionChannel ConnectionChannel;
        public MediaChannel MediaChannel;
        public HeartbeatChannel HeartbeatChannel;
        public ReceiverChannel ReceiverChannel;
        public string ChromecastApplicationId;
        public string CurrentApplicationSessionId = "";
        public string CurrentApplicationTransportId = "";
        public int CurrentMediaSessionId;

        public event EventHandler ConnectedChanged;
        public event EventHandler<ChromecastApplication> ApplicationStarted;
        public event EventHandler<MediaStatus> MediaStatusChanged;
        public event EventHandler<ChromecastStatus> ChromecastStatusChanged;
        public event EventHandler<Volume> VolumeChanged;
        public List<IChromecastChannel> Channels;

        private const string ChromecastPort = "8009";
        public CancellationTokenSource CancellationTokenSource;

        public ChromeCastClient()
        {
            ChromecastSocketService = new ChromecastSocketService();
            Channels = new List<IChromecastChannel>();
            ConnectionChannel = new ConnectionChannel(this);
            Channels.Add(ConnectionChannel);
            HeartbeatChannel = new HeartbeatChannel(this);
            Channels.Add(HeartbeatChannel);
            ReceiverChannel = new ReceiverChannel(this);
            Channels.Add(ReceiverChannel);
            MediaChannel = new MediaChannel(this);
            Channels.Add(MediaChannel);
        }


        public async Task ConnectChromecast(Uri uri)
        {
            CancellationTokenSource = new CancellationTokenSource();
            await ChromecastSocketService.Initialize(uri.Host, ChromecastPort, ConnectionChannel, HeartbeatChannel, ReadPacket, CancellationTokenSource.Token);
        }

        public async Task DisconnectChromecast()
        {
            CancellationTokenSource.Cancel();
            await ChromecastSocketService.Disconnect();
            CurrentApplicationSessionId = "";
            CurrentApplicationTransportId = "";
        }

        public IController GetControllerForCurrentApp()
        {
            var currentAppId = RunningApplication?.AppId;
            var controllerDictionary = GetControllerDictionary();
            if (currentAppId == null || controllerDictionary.Keys.Contains(currentAppId))
            {
                throw new KeyNotFoundException("No controller was found for the current applicationId");
            }

            return controllerDictionary[currentAppId];
        }

        private async void ReadPacket(Stream stream, bool parsed, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<byte> entireMessage;
                if (parsed)
                {
                    var buffer = new byte[stream.Length];
                    await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    entireMessage = buffer;
                }
                else
                {
                    entireMessage = await stream.ParseData(cancellationToken);
                }

                var entireMessageArray = entireMessage.ToArray();
                var castMessage = entireMessageArray.ToCastMessage();
                if (string.IsNullOrEmpty(castMessage?.Namespace)) return;

                Debug.WriteLine("Received: " + castMessage.GetJsonType());
                ReceivedMessage(castMessage);
            }
            catch (Exception ex)
            {
                // TODO: Catch disconnect - HResult = 0x80072745 -
                // catch this (remote device disconnect) ex = {"An established connection was aborted
                // by the software in your host machine. (Exception from HRESULT: 0x80072745)"}

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
    }
}