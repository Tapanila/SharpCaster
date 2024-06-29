using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class MediaChannelTester
    {
        private ITestOutputHelper output;
        public MediaChannelTester(ITestOutputHelper outputHelper) { 
            output = outputHelper;
        }

        [Fact]
        public async Task TestLoadingMediaQueue() {
            var chromecast = await TestHelper.FindChromecast();
            //ChromecastClient client = new ChromecastClient();
            ChromecastClient client = TestHelper.GetClientWithConsoleLogging(output, true);
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("B3419EF5");

            Item[] MyCd = new Item[4];
            MyCd[0] = new Item() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/Frankie%20by%20Mississippi%20John%20Hurt.mp3"
                },
                Autoplay = true
            };

            MyCd[1] = new Item() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/Mississippi%20Boweavil%20Blues%20by%20The%20Masked%20Marvel.mp3"
                },
                Autoplay = true
            };
            MyCd[2] = new Item() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/The%20Wild%20Wagoner%20by%20Jilson%20Setters.mp3"
                },
                Autoplay = true
            };
            MyCd[3] = new Item() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/Drunkards%20Special%20by%20Coley%20Jones.mp3"
                },
                Autoplay = true
            };

            MediaStatus status = await client.GetChannel<IMediaChannel>().QueueLoadAsync(MyCd);


            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Equal(2,status.Items.Count());           // The status message only contains the next (and if available Prev) Track/QueueItem!
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);

            // Logging Tests -> Check we only got to PLAYING once!
            Assert.NotEmpty(TestHelper.LogContent);
            Assert.Single(TestHelper.LogContent.Where((line) => line.Contains("\"playerState\":\"PLAYING\"")).ToList());


            //status = await client.GetChannel<IMediaChannel>().LoadAsync(media2, true);


        }

        [Fact]
        public async Task TestLoadingMedia()
        {
            var chromecast = await TestHelper.FindChromecast();
            //ChromecastClient client = new ChromecastClient();
            ChromecastClient client = TestHelper.GetClientWithConsoleLogging(output, true); 
            await client.ConnectChromecast(chromecast);
            _ = await client.LaunchApplicationAsync("B3419EF5");

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.GetChannel<IMediaChannel>().LoadAsync(media);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Single(status.Items);
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);

            // Logging Tests -> Check we only got to PLAYING once!
            Assert.NotEmpty(TestHelper.LogContent);
            Assert.Single(TestHelper.LogContent.Where((line)=>line.Contains("\"playerState\":\"PLAYING\"")).ToList());
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
