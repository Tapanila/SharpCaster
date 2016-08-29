using System;
using System.Linq;
using Android.App;
using Android.Widget;
using Android.OS;
using SharpCaster.Controllers;
using SharpCaster.Extensions;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;

namespace SharpCaster.Simple.Android
{
    [Activity(Label = "SharpCaster.Simple.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        static readonly ChromecastService ChromecastService = ChromecastService.Current;
        private SharpCasterDemoController _controller;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            ChromecastService.ChromeCastClient.ConnectedChanged += ChromeCastClient_ConnectedChanged;
            ChromecastService.ChromeCastClient.ApplicationStarted += ChromeCastClient_ApplicationStarted;
            var devices = await ChromecastService.StartLocatingDevices();
            ChromecastService.ConnectToChromecast(devices.First());

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }

        private async void ChromeCastClient_ApplicationStarted(object sender, Models.ChromecastStatus.ChromecastApplication e)
        {
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

            if (_controller == null)
            {
                _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();
            }

            await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", null, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });

        }

        private async void ChromeCastClient_ConnectedChanged(object sender, EventArgs e)
        {
            _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();

        }
    }
}

