using Newtonsoft.Json;
using SharpCaster.Interfaces;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastRequests;
using SharpCaster.Models.MediaStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.MediaControllers
{
    public class PlexMediaController : IMediaController
    {
        #region controller metadata
        public string DefaultAppId
        {
            get
            {
                return "CC1AD845";
            }
        }

        public string SpecificNamespace
        {
            get
            {
                return "urn:x-cast:plex";
            }
        }

        public SupportedCommand SupportedCommands
        {
            get
            {
                return SupportedCommand.Play
                    | SupportedCommand.Pause
                    | SupportedCommand.Seek;
                    //| SupportedCommand.Stop
                    //| SupportedCommand.Previous
                    //| SupportedCommand.Next
                    //| SupportedCommand.SkipTo
                    //| SupportedCommand.PlexShowDetails
                    //| SupportedCommand.PlexRefreshPlayQueue
                    //| SupportedCommand.PlexSetQuality
                    //| SupportedCommand.PlexSetStream;
            }
        }

        public bool SupportsCommand(SupportedCommand commandToCheck)
        {
            return SupportedCommands.HasFlag(commandToCheck);
        }

        #endregion

        private ChromeCastClient _chromecastClient;

        private ChromecastChannel _mediaChannel;
        private long _currentMediaSessionId;

        public PlexMediaController(ChromeCastClient chromecastClient)
        {
            _chromecastClient = chromecastClient;

            _mediaChannel = new ChromecastChannel(chromecastClient.ChromecastSocketService, this.SpecificNamespace);
            chromecastClient.Channels.Add(_mediaChannel);

            _mediaChannel.MessageReceived += MediaChannel_MessageReceived;
        }

        #region implemented commands
        public async Task Play()
        {
            await _mediaChannel.Write(PlexMediaMessageFactory.Play(_chromecastClient.CurrentApplicationTransportId, _currentMediaSessionId));
        }

        public async Task Pause()
        {
            await _mediaChannel.Write(PlexMediaMessageFactory.Pause(_chromecastClient.CurrentApplicationTransportId, _currentMediaSessionId));
        }

        public async Task Seek(double seconds)
        {
            await _mediaChannel.Write(PlexMediaMessageFactory.Seek(_chromecastClient.CurrentApplicationTransportId, _currentMediaSessionId, seconds));
        }
        #endregion

        #region not supported commands
        public async Task GetMediaStatus()
        {
            throw new NotSupportedException();
        }

        public async Task LoadSmoothStreaming(string mediaUrl, object customData = null)
        {
            throw new NotSupportedException();
        }

        public Task Shuffle(bool enabled)
        {
            throw new NotSupportedException();
        }

        public Task Repeat(RepeatMode mode)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region not implemented commands
        public Task Next()
        {
            throw new NotImplementedException();
        }

        public Task Previous()
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
        
        public Task SkipTo(object key)
        {
            throw new NotImplementedException();
        }

        public Task PlexShowDetails()
        {
            throw new NotImplementedException();
        }

        public Task PlexRefreshPlayQueue()
        {
            throw new NotImplementedException();
        }

        public Task PlexSetQuality()
        {
            throw new NotImplementedException();
        }

        public Task PlexSetStream()
        {
            throw new NotImplementedException();
        }
        #endregion



        private void MediaChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<MediaStatusResponse>(json);
            if (response.status?.Count < 1) return;
            var mediaStatus = response.status.First();

            _chromecastClient.MediaStatus = mediaStatus;
            _chromecastClient.InvokeMediaStatusChanged(mediaStatus);
            if (mediaStatus.volume != null)
            {
                _chromecastClient.UpdateVolume(mediaStatus.volume);
            }
            _currentMediaSessionId = mediaStatus.mediaSessionId;
        }

        
    }
}
