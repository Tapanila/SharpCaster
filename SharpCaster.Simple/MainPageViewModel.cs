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
        public event PropertyChangedEventHandler PropertyChanged;
        private ChromeCastClient _client;
        private CancellationTokenSource _cancellationTokenSource;
        private DeviceLocator _deviceLocator;

        public ObservableCollection<Chromecast> Chromecasts
        {
            get { return _chromecasts; }
            set
            {
                _chromecasts = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Chromecast> _chromecasts;

        public Chromecast SelectedChromecast
        {
            get { return _selectedChromecast; }
            set
            {
                _selectedChromecast = value;
                OnPropertyChanged();
            }
        }

        private Chromecast _selectedChromecast;

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
            _deviceLocator = new DeviceLocator();
            _cancellationTokenSource = new CancellationTokenSource();
            _chromecasts = new ObservableCollection<Chromecast>();
        }

        public async void StartLocating()
        {
            //We want to be alerted as soon as we found one device so we use the event
            _deviceLocator.DeviceFounded += DeviceLocator_DeviceFounded;
            await _deviceLocator.LocateDevicesAsync(_cancellationTokenSource.Token);
        }
        private async void DeviceLocator_DeviceFounded(object sender, Chromecast e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Chromecasts.Add(e);
            });
        }


        private void ConnectToChromecast(Chromecast chromecast)
        {
            _client = new ChromeCastClient();
            _client.Connected += Client_Connected;
            _client.ApplicationStarted += Client_ApplicationStarted;
            _client.ConnectChromecast(chromecast.DeviceUri);
            _client.VolumeChanged += _client_VolumeChanged;
        }

        private async void _client_VolumeChanged(object sender, Volume e)
        {
            await ExecuteOnUiThread(() => { Volume = e.level*100; });
        }

        private async void Client_ApplicationStarted(object sender, Models.ChromecastStatus.ChromecastApplication e)
        {
            await ShowMessage($"Application {e.displayName} has launched");
        }

        private async void Client_Connected(object sender, EventArgs e)
        {
            await ShowMessage("Connection established");
        }


        public async Task Connect()
        {
            if (SelectedChromecast == null)
            {
                await ShowMessage("You must first select chromecast");
                return;
            }
            ConnectToChromecast(SelectedChromecast);
        }

        public async Task LaunchApplication()
        {
            await _client.LaunchApplication("B3419EF5");
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
            if (_client.MediaStatus.PlayerState == PlayerState.Paused)
            {
                await _client.Play();
            }
            else
            {
                await _client.Pause();
            }
        }

        public async Task Pause()
        {
            await _client.Pause();
        }

        public async Task LoadMedia()
        {
            await _client.LoadMedia("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd");
        }

        public async Task Seek(double seconds)
        {
            await _client.Seek(seconds);
        }

        public async Task MuteUnmute()
        {
            await _client.SetMute(!_client.Volume.muted);
        }

        public async Task SetVolume(double newValue)
        {
            await _client.SetVolume((float) (newValue / 100));
        }
    }
}
