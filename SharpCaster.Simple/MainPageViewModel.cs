using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using SharpCaster.Models;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Simple.Annotations;

namespace SharpCaster.Simple
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        readonly ChromecastService _chromecastService = ChromecastService.Current;
        public event PropertyChangedEventHandler PropertyChanged;
        
        
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


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainPageViewModel()
        {
            _chromecastService.StartLocatingDevices();
            _chromecastService.ChromeCastClient.ApplicationStarted += Client_ApplicationStarted;
            _chromecastService.ChromeCastClient.VolumeChanged += _client_VolumeChanged;
        }

        private async void _client_VolumeChanged(object sender, Volume e)
        {
            await ExecuteOnUiThread(() => { Volume = e.level*100; });
        }

        private async void Client_ApplicationStarted(object sender, Models.ChromecastStatus.ChromecastApplication e)
        {
            await ShowMessage($"Application {e.DisplayName} has launched");
        }

        public async Task LaunchApplication()
        {
            await _chromecastService.ChromeCastClient.LaunchApplication("B3419EF5");
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
            if (_chromecastService.ChromeCastClient.MediaStatus.PlayerState == PlayerState.Paused)
            {
                await _chromecastService.ChromeCastClient.Play();
            }
            else
            {
                await _chromecastService.ChromeCastClient.Pause();
            }
        }

        public async Task Pause()
        {
            await _chromecastService.ChromeCastClient.Pause();
        }

        public async Task LoadMedia()
        {
            await _chromecastService.ChromeCastClient.LoadMedia("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd");
        }

        public async Task Seek(double seconds)
        {
            await _chromecastService.ChromeCastClient.Seek(seconds);
        }

        public async Task MuteUnmute()
        {
            await _chromecastService.ChromeCastClient.SetMute(!_chromecastService.ChromeCastClient.Volume.muted);
        }

        public async Task SetVolume(double newValue)
        {
            if (Math.Abs(_chromecastService.ChromeCastClient.Volume.level - (newValue/100)) < 0.01) return;
            await _chromecastService.ChromeCastClient.SetVolume((float) (newValue / 100));
        }

        public async Task StopApplication()
        {
            await _chromecastService.ChromeCastClient.StopApplication();
        }
    }
}
