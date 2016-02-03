using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

namespace SharpCaster.Simple
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var deviceLocator = new DeviceLocator();
            var devices = await deviceLocator.LocateDevicesAsync(TimeSpan.FromSeconds(2));
            MessageDialog msg = new MessageDialog($"Found {devices.Count()} chromecasts");
            await msg.ShowAsync();
        }
    }
}
