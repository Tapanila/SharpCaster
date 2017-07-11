using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCaster.Controllers;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;

namespace SharpCaster.Console
{
    public class ChromecastSender
    {
        static SharpCasterDemoController _controller;
        private ChromecastClient _client;

        public void Setup()
        {
            _client = new ChromecastClient();

            _client.ApplicationStarted += Client_ApplicationStarted;
            _client.VolumeChanged += _client_VolumeChanged;
            _client.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
            _client.ConnectedChanged += ChromeCastClient_Connected;
        }

        public async Task DoStuff()
        {
            

            System.Console.WriteLine("Started locating chromecasts!");
            var watcher = new ChromecastWatcher();
            var devices = await watcher.FindDevicesAsync();

            if (devices.Count == 0)
            {
                System.Console.WriteLine("No chromecasts found");
                return;
            }

            var firstChromecast = devices.First();
            System.Console.WriteLine("Device found " + firstChromecast.FriendlyName);
            _client.ConnectChromecast(firstChromecast);
        }


        private async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            System.Console.WriteLine("Connected to chromecast");
            if (_controller == null)
            {
                _controller = await _client.LaunchSharpCaster();
            }
        }

        private async void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
            if (e.PlayerState == PlayerState.Playing)
            {
                await Task.Delay(2000);
                await _client.DisconnectChromecast();
                _controller = null;
                await Task.Delay(5000);

                var watcher = new ChromecastWatcher();
                var devices = await watcher.FindDevicesAsync();

                if (devices.Count == 0)
                {
                    System.Console.WriteLine("No chromecasts found");
                    return;
                }

                var firstChromecast = devices.First();
                System.Console.WriteLine("Device found " + firstChromecast.FriendlyName);
                _client.ConnectChromecast(firstChromecast);
                await Task.Delay(5000);
                _controller = await _client.LaunchSharpCaster();
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
