using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using SharpCaster.Models;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SharpCaster.UwpControls
{
    public sealed partial class ChromecastPicker : Flyout
    {
        private readonly ChromecastWatcher _chromecastWatcher;

        public event EventHandler<ChromecastInformation> ChromecastSelected;

        public ChromecastPicker() : this("")
        {
        }

        public ChromecastPicker(string localIpAddress)
        {
            this.InitializeComponent();
            _chromecastWatcher = new ChromecastWatcher(localIpAddress);
            SetupEvents();
        }

        ~ChromecastPicker()
        {
            TeardownEvents();
        }

        private void SetupEvents()
        {
            _chromecastWatcher.Added += _chromecastWatcher_Added;
            _chromecastWatcher.Updated += _chromecastWatcher_Updated;
            _chromecastWatcher.Removed += _chromecastWatcher_Removed;
        }

        private void TeardownEvents()
        {
            _chromecastWatcher.Added -= _chromecastWatcher_Added;
            _chromecastWatcher.Updated -= _chromecastWatcher_Updated;
            _chromecastWatcher.Removed -= _chromecastWatcher_Removed;
        }

        private async void _chromecastWatcher_Removed(object sender, Models.ChromecastInformation e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                chromecasts?.Items?.Remove(e);
            });            
        }

        private async void _chromecastWatcher_Updated(object sender, Models.ChromecastInformation e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                //TODO this might be done more efficient
                foreach (var item in chromecasts?.Items)
                {
                    var chromecastInformation = item as ChromecastInformation;

                    if (chromecastInformation.SsdpUuid == e.SsdpUuid)
                    {
                        chromecastInformation.DeviceUri = e.DeviceUri;
                        chromecastInformation.FriendlyName = e.FriendlyName;
                    }
                }
            });            
        }
        
        private async void _chromecastWatcher_Added(object sender, Models.ChromecastInformation e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                chromecasts?.Items?.Add(e);
            });
        }

        private void Flyout_Opening(object sender, object e)
        {
            progressBar.IsEnabled = true;

            _chromecastWatcher.Start();
        }

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            _chromecastWatcher.Stop();
        }

        private void chromecasts_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var rSender = sender as ListView;
            if (rSender != null)
            {
                var chromecastInformation = rSender.SelectedItem as ChromecastInformation;
                ChromecastSelected?.Invoke(this, chromecastInformation);
                Hide();
            }

        }
    }
}
