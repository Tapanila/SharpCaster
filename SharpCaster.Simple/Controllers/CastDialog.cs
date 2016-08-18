using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SharpCaster.Models;
using SharpCaster.Services;

namespace SharpCaster.Simple.Controllers
{
    public sealed class CastDialog : Control
    {
        public CastDialog()
        {
            this.DefaultStyleKey = typeof(CastDialog);
        }

        public static readonly DependencyProperty ChromecastsProperty = DependencyProperty.Register(
            "Chromecasts", typeof(ObservableCollection<Chromecast>), typeof(CastDialog), new PropertyMetadata(default(ObservableCollection<Chromecast>)));

        public ObservableCollection<Chromecast> Chromecasts
        {
            get { return (ObservableCollection<Chromecast>) GetValue(ChromecastsProperty); }
            set { SetValue(ChromecastsProperty, value); }
        }

        public static readonly DependencyProperty CastButtonProperty = DependencyProperty.Register(
            "CastButton", typeof(CastButton), typeof(CastDialog), new PropertyMetadata(default(CastButton)));

        public CastButton CastButton
        {
            get { return (CastButton) GetValue(CastButtonProperty); }
            set { SetValue(CastButtonProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Margin = new Thickness { Left = Window.Current.Bounds.Width * 0.05};
            Width = Window.Current.Bounds.Width * 0.9;
            Height = Window.Current.Bounds.Height * 0.9;
            var chromecasts = GetTemplateChild("ChromecastListView") as ListView;
            chromecasts.DataContext = this;
            if (chromecasts != null) chromecasts.Tapped += Chromecast_Tapped;
        }

        private void Chromecast_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var rSender = sender as ListView;
            if (rSender != null) ChromecastService.Current.ConnectToChromecast(rSender.SelectedItem as Chromecast);
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Connecting);
            Visibility = Visibility.Collapsed;
        }
    }
}
