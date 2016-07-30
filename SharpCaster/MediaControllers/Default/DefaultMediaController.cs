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
    public class DefaultMediaController : IMediaController
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
                return DefaultMediaMessageFactory.DefaultMediaUrn;
            }
        }

        public SupportedCommand SupportedCommands
        {
            get
            {
                return SupportedCommand.GetMediaStatus
              | SupportedCommand.LoadSmoothStreaming
              | SupportedCommand.Play
              | SupportedCommand.Pause
              | SupportedCommand.Seek;
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

        public DefaultMediaController(ChromeCastClient chromecastClient)
        {
            _chromecastClient = chromecastClient;

            _mediaChannel = new ChromecastChannel(chromecastClient.ChromecastSocketService, this.SpecificNamespace);
            chromecastClient.Channels.Add(_mediaChannel);

            _mediaChannel.MessageReceived += MediaChannel_MessageReceived;
        }

        #region implemented commands
        public async Task GetMediaStatus()
        {
            await _mediaChannel.Write(DefaultMediaMessageFactory.MediaStatus(_chromecastClient.CurrentApplicationTransportId));
        }

        public async Task LoadSmoothStreaming(string mediaUrl, object customData = null)
        {
            var mediaObject = new MediaData(mediaUrl, "application/vnd.ms-sstr+xml", null, "BUFFERED", 0D, customData);
            var req = new LoadRequest(_chromecastClient.CurrentApplicationSessionId, mediaObject, true, 0.0, customData);

            var reqJson = req.ToJson();
            await _mediaChannel.Write(DefaultMediaMessageFactory.Load(_chromecastClient.CurrentApplicationTransportId, reqJson));
        }

        public async Task Play()
        {
            await _mediaChannel.Write(DefaultMediaMessageFactory.Play(_chromecastClient.CurrentApplicationTransportId, _currentMediaSessionId));
        }

        public async Task Pause()
        {
            await _mediaChannel.Write(DefaultMediaMessageFactory.Pause(_chromecastClient.CurrentApplicationTransportId, _currentMediaSessionId));
        }

        public async Task Seek(double seconds)
        {
            await _mediaChannel.Write(DefaultMediaMessageFactory.Seek(_chromecastClient.CurrentApplicationTransportId, _currentMediaSessionId, seconds));
        }
        #endregion

        #region not supported commands
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
        #endregion



        private void MediaChannel_MessageReceived(object sender, ChromecastSSLClientDataReceivedArgs e)
        {
            var json = e.Message.PayloadUtf8;
            var response = JsonConvert.DeserializeObject<MediaStatusResponse>(json);
            if (response.status?.Count < 1) return;
            var mediaStatus = response.status.First();

            // TODO shouldn't the MediaStatus property and the MediaStatusChanged event
            // be a part of IMediaController instead of ChromeCastClient?
            _chromecastClient.MediaStatus = mediaStatus;
            _chromecastClient.InvokeMediaStatusChanged(mediaStatus);

            //TODO According to https://developers.google.com/cast/docs/reference/messages#SetVolume
            // There is a difference between device volume and stream volume.
            // Since this property is part of IMediaController it should be used
            // only for stream volume!
            //Because of that I commented it out for now.
            //if (mediaStatus.volume != null)
            //{
            //    _chromecastClient.UpdateVolume(mediaStatus.volume);
            //}
            _currentMediaSessionId = mediaStatus.mediaSessionId;
        }
    }
}
