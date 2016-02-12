using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SharpCaster.Models;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using SharpCaster.Simple.Annotations;

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

        private async void Play(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.Play();
        }

        private async void Pause(object sender, RoutedEventArgs e)
        {
            await MainPageViewModel.Pause();
        }
    }
}
