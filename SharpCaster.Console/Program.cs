using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

            DoStuff();
            var input = System.Console.ReadLine();
        }

        private static async Task DoStuff()
        {
            ChromecastService.ChromeCastClient.ApplicationStarted += Client_ApplicationStarted;
            ChromecastService.ChromeCastClient.VolumeChanged += _client_VolumeChanged;
            ChromecastService.ChromeCastClient.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
            ChromecastService.ChromeCastClient.ConnectedChanged += ChromeCastClient_Connected;

            System.Console.WriteLine("Started locating chromecasts!");
            var devices = await ChromecastService.StartLocatingDevices();

            if (devices.Count == 0)
            {
                System.Console.WriteLine("No chromecasts found");
                return;
            }

            var firstChromecast = devices.First();
            System.Console.WriteLine("Device found " + firstChromecast.FriendlyName);
            ChromecastService.ConnectToChromecast(firstChromecast);
    }


        private static async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            System.Console.WriteLine("Connected to chromecast");
            if (_controller == null)
            {
                _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();
            }
        }

        private async static void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
            if (e.PlayerState == PlayerState.Playing)
            {
                await Task.Delay(2000);
                await ChromecastService.ChromeCastClient.DisconnectChromecast();
                _controller = null;
                await Task.Delay(5000);
                var devices = await ChromecastService.StartLocatingDevices();

                if (devices.Count == 0)
                {
                    System.Console.WriteLine("No chromecasts found");
                    return;
                }

                var firstChromecast = devices.First();
                System.Console.WriteLine("Device found " + firstChromecast.FriendlyName);
                ChromecastService.ConnectToChromecast(firstChromecast);
                await Task.Delay(5000);
                _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();
                await Task.Delay(4000);
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
                while (_controller == null)
                {
                    await Task.Delay(500);
                }

                await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", null, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });
            }
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
            while (_controller == null)
            {
                await Task.Delay(500);
            }

            await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", null, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });
        }
    }
}
