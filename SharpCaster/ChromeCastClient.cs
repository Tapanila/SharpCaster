using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpCaster.Channels;
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
                if (_connected != value) ConnectedChanged?.Invoke(this, EventArgs.Empty);
                _connected = value;
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

        public IChromecastSocketService ChromecastSocketService {get; set; }

        public ConnectionChannel ConnectionChannel;
        public MediaChannel MediaChannel;
        public HeartbeatChannel HeartbeatChannel;
        public ReceiverChannel ReceiverChannel;
        private const string ChromecastPort = "8009";
        public string ChromecastApplicationId;
        public string CurrentApplicationSessionId = "";
        public string CurrentApplicationTransportId = "";
        public long CurrentMediaSessionId;
        
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
            ConnectionChannel = new ConnectionChannel(this);
            Channels.Add(ConnectionChannel);
            HeartbeatChannel = new HeartbeatChannel(this);
            Channels.Add(HeartbeatChannel);
            ReceiverChannel = new ReceiverChannel(this);
            Channels.Add(ReceiverChannel);
            MediaChannel = new MediaChannel(this);
            Channels.Add(MediaChannel);
        }
        
        
        public async void ConnectChromecast(Uri uri)
        {
            await ChromecastSocketService.Initialize(uri.Host, ChromecastPort, ConnectionChannel, HeartbeatChannel, ReadPacket);
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
                if (string.IsNullOrEmpty(castMessage?.Namespace)) return;
                Debug.WriteLine("Received: " + castMessage.GetJsonType());
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
    }
}
