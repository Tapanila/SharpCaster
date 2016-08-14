using System;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;

namespace SharpCaster.Console
{
    class Program
    {
        static readonly ChromecastService ChromecastService = ChromecastService.Current;
        
        static void Main(string[] args)
        {
            
#pragma warning disable 4014
            ChromecastService.StartLocatingDevices();
            System.Console.WriteLine("Started locating chromecasts!");
#pragma warning restore 4014
            ChromecastService.DeviceLocator.DeviceFound += DeviceLocator_DeviceFound;
            ChromecastService.ChromeCastClient.ApplicationStarted += Client_ApplicationStarted;
            ChromecastService.ChromeCastClient.VolumeChanged += _client_VolumeChanged;
            ChromecastService.ChromeCastClient.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
            ChromecastService.ChromeCastClient.ConnectedChanged += ChromeCastClient_Connected;

            var input = System.Console.ReadLine();
        }

        private static void DeviceLocator_DeviceFound(object sender, Chromecast e)
        {
            if (!e.FriendlyName.Contains("CC"))
            {
                return;
            }
            ChromecastService.StopLocatingDevices();
            System.Console.WriteLine("Device found " + e.FriendlyName);
            ChromecastService.ConnectToChromecast(e);
        }

        private static async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            await ChromecastService.ChromeCastClient.ConnectionChannel.LaunchApplication("B3419EF5");
            System.Console.WriteLine("Connected to chromecast");
        }

        private static void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
        }

        private static void _client_VolumeChanged(object sender, Volume e)
        {
        }

        private static async void Client_ApplicationStarted(object sender, ChromecastApplication e)
        {
            System.Console.WriteLine($"Application {e.DisplayName} has launched");
            await ChromecastService.ChromeCastClient.MediaChannel.LoadMedia("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd");
        }
    }
}
