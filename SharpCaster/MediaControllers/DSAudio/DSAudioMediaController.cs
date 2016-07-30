using Newtonsoft.Json;
using SharpCaster.Models;
using SharpCaster.Models.MediaStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.MediaControllers
{
    public class DSAudioMediaController : IMediaController
    {
        public string DefaultAppId
        {
            get
            {
                return "ED01B6D7";
            }
        }

        public string SpecificNamespace
        {
            get
            {
                return "urn:x-cast:com.synology.dsaudio";
            }
        }

        public SupportedCommand SupportedCommands
        {
            get
            {
                return SupportedCommand.SkipTo
                    | SupportedCommand.Pause
                    | SupportedCommand.Play
                    | SupportedCommand.Stop
                    | SupportedCommand.Next
                    | SupportedCommand.Previous
                    | SupportedCommand.Seek
                    | SupportedCommand.Repeat
                    | SupportedCommand.Shuffle
                    | SupportedCommand.DSAudioReplayCurrent
                    | SupportedCommand.DSAudioUpdatePlaylist;
            }
        }

        private ChromeCastClient _chromecastClient;

        private ChromecastChannel _mediaChannel;
        private long _currentMediaSessionId;

        public DSAudioMediaController(ChromeCastClient chromecastClient)
        {
            _chromecastClient = chromecastClient;

            _mediaChannel = new ChromecastChannel(chromecastClient.ChromecastSocketService, this.SpecificNamespace);
            chromecastClient.Channels.Add(_mediaChannel);

            _mediaChannel.MessageReceived += MediaChannel_MessageReceived;
        }

        private async Task Identify(string authKey = null)
        {
            var message = new
            {
                command = "identify",
                auth_key = authKey
            };

        }

        #region not supported
        public Task GetMediaStatus()
        {
            throw new NotSupportedException();
        }

        public Task LoadSmoothStreaming(string streamUrl, object customData = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        public Task Next()
        {
            var message = new
            {
                command = "next"
            };
            throw new NotImplementedException();
        }

        public Task Pause()
        {
            var message = new
            {
                command = "pause"
            };
            throw new NotImplementedException();
        }

        public Task Play()
        {
            var message = new
            {
                command = "resume"
            };
            throw new NotImplementedException();
        }

        public Task Previous()
        {
            var message = new
            {
                command = "prev"
            };
            throw new NotImplementedException();
        }

        public Task Repeat(RepeatMode mode)
        {
            var message = new
            {
                command = "set_repeat",
                mode = mode.ToString().ToLower()
            };
            throw new NotImplementedException();
        }

        public Task Seek(double seconds)
        {
            var message = new
            {
                command = "seek",
                position = seconds
            };
            throw new NotImplementedException();
        }

        public Task Shuffle(bool enabled)
        {
            var message = new
            {
                command = "set_shuffle",
                enabled = enabled
            };
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            var message = new
            {
                command = "stop"
            };
            throw new NotImplementedException();
        }

        public Task DSAudioReplayCurrent()
        {
            var message = new
            {
                command = "replayCurrent"
            };
            throw new NotImplementedException();
        }

        public Task DSAudioUpdatePlaylist(object offset, object limit, List<object> songs, int playingIndex)
        {
            var message = new
            {
                command = "update_playlist",
                offset = offset,
                limit = limit,
                songs = songs,
                playing_index = playingIndex
            };
            throw new NotImplementedException();
        }

        public bool SupportsCommand(SupportedCommand commandToCheck)
        {
            throw new NotImplementedException();
        }

        //TODO this controller uses a different MediaStatus format
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
