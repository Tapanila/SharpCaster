using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SharpCaster.Simple
{
    public sealed partial class MainPage : Page
    { 
        public MainPageViewModel MainPageViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();
            MainPageViewModel = new MainPageViewModel();
            DataContext = MainPageViewModel;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainPageViewModel.StartLocating();
        }

        private async void ConnectClicked(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.Connect();
        }

        private async void LaunchApplication(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.LaunchApplication();
        }

        private async void LoadMedia(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.LoadMedia();
        }

        private async void PlayPause(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.PlayPause();
        }

        private async void Seek(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.Seek(90);
        }

        private async void MuteUnmute(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.MuteUnmute();
        }

        private async void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            await MainPageViewModel.SetVolume(e.NewValue);
        }

        private async void StopApplication(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.StopApplication();
        }
    }
}
