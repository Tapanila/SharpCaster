using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
using SharpCaster.Models;

namespace SharpCaster.Simple.Controllers
{
    public class CastButton : Control
    {
        public static readonly DependencyProperty CastDialogProperty = DependencyProperty.Register("CastDialog", typeof(CastDialog), typeof(CastButton), new PropertyMetadata(null));

        public CastDialog CastDialog
        {
            get { return (CastDialog) GetValue(CastDialogProperty); }
            set { SetValue(CastDialogProperty, value); }
        }

        private Popup _popup;

        public static readonly DependencyProperty ConnectedToChromecastProperty = DependencyProperty.Register(
            "ConnectedToChromecast", typeof(bool), typeof(CastButton), new PropertyMetadata(default(bool), ConnectionStateChanged));

        private static void ConnectionStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var rObject = dependencyObject as CastButton;
            rObject?.UpdateState();
        }

        public bool ConnectedToChromecast
        {
            get { return (bool) GetValue(ConnectedToChromecastProperty); }
            set { SetValue(ConnectedToChromecastProperty, value); }
        }

        public static readonly DependencyProperty ChromecastsProperty = DependencyProperty.Register(
            "Chromecasts", typeof(ObservableCollection<Chromecast>), typeof(CastButton), new PropertyMetadata(default(ObservableCollection<Chromecast>)));

        public ObservableCollection<Chromecast> Chromecasts
        {
            get { return (ObservableCollection<Chromecast>) GetValue(ChromecastsProperty); }
            set { SetValue(ChromecastsProperty, value); }
        }
        
        public CastButton()
        {
            DefaultStyleKey = typeof(CastButton);
        }

        private async void UpdateState()
        {
            await ExecuteOnUiThread(() =>
            {
                if (ConnectedToChromecast)
                {
                    VisualStateManager.GoToState(this, CastButtonVisualStates.InteractiveStates.Connected.ToString(),
                        true);
                    return;
                }
                if (Chromecasts?.Count > 0)
                {
                    VisualStateManager.GoToState(this, CastButtonVisualStates.InteractiveStates.Disconnected.ToString(),
                        true);
                    return;
                }
                VisualStateManager.GoToState(this, CastButtonVisualStates.InteractiveStates.Unavailable.ToString(), true);
            });
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var castIcon = GetTemplateChild("CastIcon") as Path;
            if (castIcon != null) Tapped += CastIcon_Tapped;
            Chromecasts.CollectionChanged += Chromecasts_CollectionChanged;
            UpdateState();
        }

        private void Chromecasts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateState();
        }

        private void CastIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (CastDialog == null) CastDialog = new CastDialog {Chromecasts = Chromecasts, CastButton = this};
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

        public void GoToState(CastButtonVisualStates.InteractiveStates state)
        {
            VisualStateManager.GoToState(this, state.ToString(), true);           
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
