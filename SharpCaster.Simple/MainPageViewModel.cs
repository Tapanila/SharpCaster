using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using SharpCaster.Models;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;
using SharpCaster.Simple.Annotations;

namespace SharpCaster.Simple
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        readonly ChromecastService _chromecastService = ChromecastService.Current;
        public event PropertyChangedEventHandler PropertyChanged;
        private DispatcherTimer secondsTimer;

        public ChromecastService ChromecastService => _chromecastService;

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
#pragma warning disable 4014
            _chromecastService.StartLocatingDevices();
#pragma warning restore 4014
            _chromecastService.ChromeCastClient.ApplicationStarted += Client_ApplicationStarted;
            _chromecastService.ChromeCastClient.VolumeChanged += _client_VolumeChanged;
            _chromecastService.ChromeCastClient.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
            _chromecastService.ChromeCastClient.Connected += ChromeCastClient_Connected;
            secondsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            secondsTimer.Tick += SecondsTimer_Tick;
        }

        private async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            await _chromecastService.ChromeCastClient.LaunchApplication("B3419EF5");
        }

        private void SecondsTimer_Tick(object sender, object e)
        {
            Position += 1;
        }

        private async void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
            await ExecuteOnUiThread(() =>
            {
                switch (_chromecastService.ChromeCastClient.MediaStatus.PlayerState)
                {
                    case PlayerState.Playing:
                        secondsTimer.Start();
                        break;
                    default:
                        secondsTimer.Stop();
                        break;
                }
                Position = _chromecastService.ChromeCastClient.MediaStatus.currentTime;
                if (_chromecastService.ChromeCastClient.MediaStatus.media != null)
                    Length = _chromecastService.ChromeCastClient.MediaStatus.media.duration;
            });
        }

        private async void _client_VolumeChanged(object sender, Volume e)
        {
            await ExecuteOnUiThread(() => { Volume = e.level * 100; });
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
            var mediaController = _chromecastService.ChromeCastClient.MediaController;

            if (_chromecastService.ChromeCastClient.MediaStatus != null && _chromecastService.ChromeCastClient.MediaStatus.PlayerState == PlayerState.Paused)
            {
                if (mediaController.SupportsCommand(MediaControllers.SupportedCommand.Play))
                {
                    await mediaController.Play();
                }
            }
            else
            {
                if (mediaController.SupportsCommand(MediaControllers.SupportedCommand.Pause))
                {
                    await mediaController.Pause();
                }
            }
        }

        public async Task Pause()
        {
            var mediaController = _chromecastService.ChromeCastClient.MediaController;

            if (mediaController.SupportsCommand(MediaControllers.SupportedCommand.Pause))
            {
                await mediaController.Pause();
            }
        }

        public async Task LoadMedia(string title, string description, ImageSource poster)
        {
            var mediaController = _chromecastService.ChromeCastClient.MediaController;

            if (mediaController.SupportsCommand(MediaControllers.SupportedCommand.LoadSmoothStreaming))
            {
                Title = title;
                Description = description;
                Poster = poster;
                await mediaController.LoadSmoothStreaming("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd");
            }
        }

        public async Task Seek(double seconds)
        {
            var mediaController = _chromecastService.ChromeCastClient.MediaController;

            if (Math.Abs(Position - seconds) > 0.1)
            {
                if (mediaController.SupportsCommand(MediaControllers.SupportedCommand.Seek))
                {
                    await mediaController.Seek(seconds);
                }
            }
        }

        public async Task MuteUnmute()
        {
            await _chromecastService.ChromeCastClient.SetMute(!_chromecastService.ChromeCastClient.Volume.muted);
        }

        public async Task SetVolume(double newValue)
        {
            if (Math.Abs(_chromecastService.ChromeCastClient.Volume.level - (newValue / 100)) < 0.01) return;
            await _chromecastService.ChromeCastClient.SetVolume((float)(newValue / 100));
        }

        public async Task StopApplication()
        {
            await _chromecastService.ChromeCastClient.StopApplication();
        }
    }
}
