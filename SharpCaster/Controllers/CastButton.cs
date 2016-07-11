using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
using SharpCaster.Services;

namespace SharpCaster.Controllers
{
    public class CastButton : Control
    {
        public static readonly DependencyProperty ChromecastServiceProperty = DependencyProperty.Register("ChromecastService", typeof(ChromecastService), typeof(CastButton), new PropertyMetadata(null));

        public ChromecastService ChromecastService
        {
            get { return (ChromecastService)GetValue(ChromecastServiceProperty); }
            set { SetValue(ChromecastServiceProperty, value); }
        }

        public static readonly DependencyProperty CastDialogProperty = DependencyProperty.Register("CastDialog", typeof(CastDialog), typeof(CastButton), new PropertyMetadata(null));

        public CastDialog CastDialog
        {
            get { return (CastDialog) GetValue(CastDialogProperty); }
            set { SetValue(CastDialogProperty, value); }
        }

        private Popup _popup;

        
        public CastButton()
        {
            DefaultStyleKey = typeof(CastButton);
        }

        private void UpdateState()
        {
            if (ChromecastService?.ConnectedChromecast != null)
            {
                VisualStateManager.GoToState(this, CastButtonVisualStates.InteractiveStates.Connected.ToString(), true);
                return;
            }
            if (ChromecastService?.DeviceLocator.DiscoveredDevices.Count > 0)
            {
                VisualStateManager.GoToState(this, CastButtonVisualStates.InteractiveStates.Disconnected.ToString(), true);
                return;
            }
            VisualStateManager.GoToState(this, CastButtonVisualStates.InteractiveStates.Unavailable.ToString(), true);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var castIcon = GetTemplateChild("CastIcon") as Path;
            if (ChromecastService != null)
            {
                ChromecastService.CastButton = this;
            }
            if (castIcon != null) Tapped += CastIcon_Tapped;
            UpdateState();
        }

        private void CastIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (CastDialog == null) CastDialog = new CastDialog {ChromecastService = ChromecastService};
            if (_popup != null)
            {
                CastDialog.Visibility = Visibility.Visible;
                return;
            }
            _popup = new Popup
            {
                Child = CastDialog,
                IsOpen = true,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }
        

        private static async Task ExecuteOnUiThread(DispatchedHandler yourAction)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, yourAction);
        }

        public async Task GoToState(CastButtonVisualStates.InteractiveStates state)
        {
            await ExecuteOnUiThread(() =>
            {
                VisualStateManager.GoToState(this, state.ToString(), true);
            });
            
        }
    }
    public static class CastButtonVisualStates
    {
        internal static class GroupNames
        {
            internal const string InteractiveStates = "InteractiveStates";
        }

        public enum InteractiveStates
        {
            Unavailable,
            Disconnected,
            Connecting,
            Connected
        }
    }
}
