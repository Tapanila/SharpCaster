using System;
using SharpCaster.Controllers;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;

namespace SharpCaster.Console
{
    class Program
    {
        static readonly ChromecastService ChromecastService = ChromecastService.Current;
        static SharpCasterDemoController _controller;
        
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
            ChromecastService.StopLocatingDevices();
            System.Console.WriteLine("Device found " + e.FriendlyName);
            ChromecastService.ConnectToChromecast(e);
        }

        private static async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();
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
            var track = new Track
            {
                Name = "English Subtitle",
                TrackId = 100,
                Type = "TEXT",
                SubType = "captions",
                Language = "en-US",
                TrackContentId =
               "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/tracks/DesigningForGoogleCast-en.vtt"
            };


            await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", null, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });
        }
    }
}
