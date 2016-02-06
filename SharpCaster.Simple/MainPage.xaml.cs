using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Threading;
using SharpCaster.Models;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace SharpCaster.Simple
{
    public sealed partial class MainPage : Page
    {
        private ChromeCastClient _client;
        private CancellationTokenSource _cancellationTokenSource;
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var deviceLocator = new DeviceLocator();
            _cancellationTokenSource = new CancellationTokenSource();
            //We want to be alerted as soon as we found one device so we use the event
            deviceLocator.DeviceListChanged += DeviceLocator_DeviceListChanged;
            await deviceLocator.LocateDevicesAsync(_cancellationTokenSource.Token);  
        }

        private async void DeviceLocator_DeviceListChanged(object sender, System.Collections.Generic.List<Chromecast> e)
        {
            //We are only looking for one chromecast so we cancel the process after finding it
            _cancellationTokenSource.Cancel();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog msg = new MessageDialog("Found chromecast " + e.First().FriendlyName);
                    await msg.ShowAsync();
                });
            StartApplication(e.First());
        }

        private void StartApplication(Chromecast chromecast)
        {
            _client = new ChromeCastClient();
            _client.Connected += Client_Connected;
            _client.ApplicationStarted += Client_ApplicationStarted;
            _client.ConnectChromecast(chromecast.DeviceUri);
        }

        private async void Client_ApplicationStarted(object sender, Models.ChromecastStatus.ChromecastApplication e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                    {
                        var msg = new MessageDialog($"Application {e.displayName} has launched");
                        await msg.ShowAsync();
                });
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            _client.LaunchApplication("B3419EF5");
        }
    }
}
