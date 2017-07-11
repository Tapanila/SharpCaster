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

namespace SharpCaster.UwpControls
{
    public class CastButton : Control
    {
        public static readonly DependencyProperty ConnectedProperty = DependencyProperty.Register(
            "Connected", typeof(bool), typeof(CastButton), new PropertyMetadata(default(bool)));

        public bool Connected
        {
            get { return (bool)GetValue(ConnectedProperty); }
            set
            {
                if (value)
                {
                    GoToState(CastButtonVisualStates.InteractiveStates.Connected);
                }
                else
                {
                    GoToState(CastButtonVisualStates.InteractiveStates.Disconnected);
                }

                SetValue(ConnectedProperty, value);
            }
        }

        public event EventHandler<ChromecastInformation> ChromecastSelected;

        private readonly ChromecastPicker _castPicker;

        public CastButton()
        {
            _castPicker = new ChromecastPicker();
            _castPicker.ChromecastSelected += CastPicker_ChromecastSelected;

            DefaultStyleKey = typeof(CastButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var castIcon = GetTemplateChild("CastIcon") as Path;
            if (castIcon != null) Tapped += CastIcon_Tapped;
        }

        private void CastIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var castButton = sender as CastButton;
            if (castButton != null)
            {
                _castPicker.ShowAt(castButton);
            }
        }

        private void CastPicker_ChromecastSelected(object sender, ChromecastInformation e)
        {
            ChromecastSelected?.Invoke(this, e);

            GoToState(CastButtonVisualStates.InteractiveStates.Connecting);
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
