using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using SharpCaster.Controllers;
using SharpCaster.Models;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Simple.Annotations;
using SharpCaster.Models.Metadata;

namespace SharpCaster.Simple
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DispatcherTimer secondsTimer;
        private SharpCasterDemoController _controller;

        public ChromecastInformation _chromecast { get; set; }
        public ChromecastInformation Chromecast { get { return _chromecast; }
            set
            {
                if (_chromecastClient != null)
                {
                    _chromecastClient.DisconnectChromecast();
                }

                _chromecast = value;

                _chromecastClient = new ChromecastClient();
                _chromecastClient.ApplicationStarted += Client_ApplicationStarted;
                _chromecastClient.VolumeChanged += _client_VolumeChanged;
                _chromecastClient.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
                _chromecastClient.ConnectedChanged += ChromeCastClient_Connected;
                _chromecastClient.ConnectChromecast(_chromecast);
            } }

        private ChromecastClient _chromecastClient { get; set; }
                
        public bool ConnectedToChromecast
        {
            get
            {
                return _connectedToChromecast;
            }
            set
            {
                _connectedToChromecast = value;
                OnPropertyChanged();
            }
        }

        private bool _connectedToChromecast;

        public double Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                OnPropertyChanged();
            }
        }

        private double _volume;

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                OnPropertyChanged();
            }
        }

        private double _length;

        public double Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        private double _position;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _title;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _description;

        public ImageSource Poster
        {
            get { return _poster; }
            set
            {
                _poster = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _poster;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainPageViewModel()
        {
            secondsTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            secondsTimer.Tick += SecondsTimer_Tick;
        }
        

        
        private async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            await ExecuteOnUiThread(() =>
            {
                ConnectedToChromecast = true;                
            });
            _controller = await _chromecastClient.LaunchSharpCaster();
        }

        private void SecondsTimer_Tick(object sender, object e)
        {
            Position += 1;
        }

        private async void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
            await ExecuteOnUiThread(() =>
            {
                switch (e.PlayerState)
                {
                    case PlayerState.Playing:
                        secondsTimer.Start();
                        break;
                    default:
                        secondsTimer.Stop();
                        break;
                }
                Position = e.CurrentTime;
                if (e.Media != null)
                    Length = e.Media.duration;
            });
        }

        private async void _client_VolumeChanged(object sender, Volume e)
        {
            await ExecuteOnUiThread(() => { Volume = e.level*100; });
        }

        private async void Client_ApplicationStarted(object sender, Models.ChromecastStatus.ChromecastApplication e)
        {
            await ShowMessage($"Application {e.DisplayName} has launched");
        }

        private async Task ShowMessage(string message)
        {
            await ExecuteOnUiThread(
            async () =>
            {
                var msg = new MessageDialog(message);
                await msg.ShowAsync();
            });
        }

        private static async Task ExecuteOnUiThread(DispatchedHandler yourAction)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, yourAction);
        }

        public async Task PlayPause()
        {
            if (_chromecastClient?.MediaStatus != null && _chromecastClient?.MediaStatus.PlayerState == PlayerState.Paused)
            {
                await _controller.Play();
            }
            else
            {
                await _controller.Pause();
            }
        }

        public async Task Pause()
        {
            await _controller.Pause();
        }

        public async Task LoadMedia(string title, string description, ImageSource poster)
        {
            Title = title;
            Description = description;
            Poster = poster;
            var track = new Track
            {
                Name = "English Subtitle",
                TrackId = 100,
                Type = "TEXT",
                SubType = "captions",
                Language = "en-US",
                TrackContentId =
                    "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/tracks/DesigningForGoogleCast-en.vtt"
            };

            var metadata = new GenericMediaMetadata { title = title};

            await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", metadata, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });
        }

        public async Task Seek(double seconds)
        {
            if (Math.Abs(Position - seconds) > 0.1)
            await _controller.Seek(seconds);
        }

        public async Task MuteUnmute()
        {
            await _controller.SetMute(!_chromecastClient.Volume.muted);
        }

        public async Task SetVolume(double newValue)
        {
            if (Math.Abs(_chromecastClient.Volume.level - (newValue/100)) < 0.01) return;
            await _controller.SetVolume((float) (newValue / 100));
        }

        public async Task StopApplication()
        {
            await _controller.StopApplication();
        }
    }
}
