using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public IChromecastSocketService ChromecastSocketService {get; set; }

        public IChromecastChannel _connectionChannel;
        public MediaChannel _mediaChannel;
        public HeartbeatChannel _heartbeatChannel;
        public ReceiverChannel _receiverChannel;
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

            _connectionChannel = CreateChannel(MessageFactory.DialConstants.DialConnectionUrn);
            _heartbeatChannel = new HeartbeatChannel(this);
            Channels.Add(_heartbeatChannel);
            _receiverChannel = new ReceiverChannel(this);
            Channels.Add(_receiverChannel);
            _mediaChannel = new MediaChannel(this);
            Channels.Add(_mediaChannel);
        }

        private IChromecastChannel CreateChannel(string ns)
        {
            var channel = new DefaultChannel(this,ns);
            Channels.Add(channel);
            return channel;
        }



       

    


        public async Task<bool> ConnectToApplication(string applicationId)
        {
            var startedApplication = ChromecastStatus?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
            if (startedApplication == null) return false;
            if (!string.IsNullOrWhiteSpace(CurrentApplicationSessionId)) return false;
            CurrentApplicationSessionId = startedApplication.SessionId;
            CurrentApplicationTransportId = startedApplication.TransportId;
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
            await Write(MessageFactory.MediaStatus(CurrentApplicationTransportId).ToProto());
        }

        public async Task LaunchApplication(string applicationId, bool joinExisting = true)
        {
            ChromecastApplicationId = applicationId;
            if (joinExisting && await ConnectToApplication(applicationId))
            {
                await GetMediaStatus();
                return;
            }
            await Write(MessageFactory.Launch(applicationId).ToProto());
        }

        public async Task StopApplication()
        {
            await Write(MessageFactory.Stop(CurrentApplicationSessionId).ToProto());
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
