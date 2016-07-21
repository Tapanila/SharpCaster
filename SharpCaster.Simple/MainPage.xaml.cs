using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;

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
        }

        private async void LoadMedia(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.LoadMedia(
                "Big Buck Bunny", 
                "A large and lovable rabbit", 
                new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/339px-Big_buck_bunny_poster_big.jpg")));
        }

        private async void PlayPause(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.PlayPause();
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
        

        private async void TimelineValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            await MainPageViewModel.Seek(e.NewValue);
        }
    }
}
