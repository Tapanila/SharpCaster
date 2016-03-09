using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SharpCaster.Models;

namespace SharpCaster.Controllers
{
    public sealed class CastDialog : Control
    {
        public static readonly DependencyProperty ChromecastServiceProperty = DependencyProperty.Register(
            "ChromecastService", typeof (ChromecastService), typeof (CastDialog), new PropertyMetadata(default(ChromecastService)));

        public ChromecastService ChromecastService
        {
            get { return (ChromecastService) GetValue(ChromecastServiceProperty); }
            set { SetValue(ChromecastServiceProperty, value); }
        }
        public CastDialog()
        {
            this.DefaultStyleKey = typeof(CastDialog);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Margin = new Thickness { Left = Window.Current.Bounds.Width * 0.05};
            Width = Window.Current.Bounds.Width * 0.9;
            Height = Window.Current.Bounds.Height * 0.9;
            var chromecasts = GetTemplateChild("ChromecastListView") as ListView;
            if (chromecasts != null) chromecasts.Tapped += Chromecast_Tapped;
        }

        private void Chromecast_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var rSender = sender as ListView;
            if (rSender != null) ChromecastService.ConnectToChromecast(rSender.SelectedItem as Chromecast);
            Visibility = Visibility.Collapsed;
        }
    }
}
