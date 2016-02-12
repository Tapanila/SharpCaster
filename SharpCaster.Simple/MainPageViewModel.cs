using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using SharpCaster.Models;
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

        private async void _client_VolumeChanged(object sender, float e)
        {
            await ShowMessage("Chromecast volume is now " + e);
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
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {

                var msg = new MessageDialog(message);
                await msg.ShowAsync();
            });
        }

        public async Task Play()
        {
            await _client.PlayMedia("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd");
        }

        public async Task Pause()
        {
            await _client.PauseMedia();
        }
    }
}
