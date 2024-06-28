using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class MediaChannelTester
    {
        [Fact]
        public async Task TestLoadingMedia()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("B3419EF5");

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };
            _ = await client.GetChannel<IMediaChannel>().LoadAsync(media);
        }

        [Fact]
        public async Task StartApplicationAThenStartBAndLoadMedia()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("A9BCCB7C", false);

            await client.DisconnectAsync();
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("B3419EF5");
            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };
            _ = await client.GetChannel<IMediaChannel>().LoadAsync(media);
        }

        [Fact]
        public async Task TestLoadingAndPausingMedia()
        {
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.LaunchApplicationAsync("B3419EF5", false);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            String runSequence = "R";

            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.GetChannel<IMediaChannel>().StatusChanged += async (object sender, EventArgs e) =>
            {
                //runSequence += ".";
                if (client.GetChannel<IMediaChannel>().Status.FirstOrDefault()?.PlayerState == PlayerStateType.Playing)
                {
                    runSequence += "p";
                    mediaStatus = await client.GetChannel<IMediaChannel>().PauseAsync();
                    Assert.Equal(PlayerStateType.Paused, mediaStatus.PlayerState);
                    runSequence += "P";
                    _autoResetEvent.Set();
                } 
            };

            runSequence += "1";
            mediaStatus = await client.GetChannel<IMediaChannel>().LoadAsync(media);
            runSequence += "2";

            //This checks that within 5000 ms we have loaded video and were able to pause it
            Assert.True(_autoResetEvent.WaitOne(5000));
            runSequence += "3";

            Assert.Equal("R1p2P3", runSequence);
        }

        [Fact]
        public async Task TestLoadingAndStoppingMedia()
        {
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            await client.ConnectChromecast(chromecast);

            var status = await client.LaunchApplicationAsync("B3419EF5",false);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.GetChannel<IMediaChannel>().StatusChanged += async (object sender, EventArgs e) =>
            {
                if (client.GetChannel<IMediaChannel>().Status.FirstOrDefault()?.PlayerState == PlayerStateType.Playing)
                {
                    mediaStatus = await client.GetChannel<IMediaChannel>().StopAsync();
                    _autoResetEvent.Set();
                }
            };

            mediaStatus = await client.GetChannel<IMediaChannel>().LoadAsync(media);

            //This checks that within 5000 ms we have loaded video and were able to pause it
            Assert.True(_autoResetEvent.WaitOne(5000));
        }
    }
}
