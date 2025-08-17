using Sharpcaster.Models;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Sharpcaster.Test.helper;
using Sharpcaster.Models.Media;
using System.Linq;
using System.Collections.Generic;

namespace Sharpcaster.Test
{
    public class ChromecastConnectionTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task SearchChromecastsAndConnectToIt()
        {
            var TestHelper = new TestHelper();
            var status = await TestHelper.CreateAndConnectClient(outputHelper, fixture.Receivers[0]);
            Assert.NotNull(status);
        }

        [Fact(Skip = "Test needs manuell interactions -> skipped for autotestings")]
        public async Task SearchChromecastsAndConnectToItThenWaitForItToShutdown()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            Assert.NotNull(client.ChromecastStatus);
            AutoResetEvent _autoResetEvent = new(false);

            client.Disconnected += (sender, args) =>
            {
                outputHelper.WriteLine("Chromecast did shutdown");
                _autoResetEvent.Set();
            };

            //This checks that within 30 seconds we have noticed that device was turned off
            //This need manual intervention to turn off the device
            outputHelper.WriteLine("Waiting for Chromecast to shutdown");
            Assert.True(_autoResetEvent.WaitOne(30000), "This test fails if run without manunal intervention on the used cast devide!");
        }

        [Fact]
        public async Task TestingHeartBeat()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);
            AutoResetEvent _autoResetEvent = new(false);

            //We are going to load video & start playing it
            //Then we are going to pause and play it quite many times
            //During this heartbeat timeout would have been triggered without previous fix

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus mediaStatus;
            string runSequence = "Load";
            string play = "Play";
            string pause = "Pause";
            int commandsToRun = 10;

            //We are setting up an event to listen to status change. Because we don't know when the video has started to play
            client.MediaChannel.StatusChanged += (object sender, MediaStatus e) =>
            {
                _autoResetEvent.Set();
            };

            mediaStatus = await client.MediaChannel.LoadAsync(media);

            _autoResetEvent.WaitOne(3000);

            for (int i = 0; i < commandsToRun; i++)
            {
                if (i % 2 == 0)
                {
                    await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);
                    mediaStatus = await client.MediaChannel.PauseAsync();
                    Assert.Equal(PlayerStateType.Paused, mediaStatus.PlayerState);
                    runSequence += pause;
                }
                else
                {
                    await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);
                    mediaStatus = await client.MediaChannel.PlayAsync();
                    Assert.Equal(PlayerStateType.Playing, mediaStatus.PlayerState);
                    runSequence += play;
                }
            }

            string expectedSequence = "Load" + string.Concat(Enumerable.Repeat(pause + play, commandsToRun / 2));

            Assert.Equal(expectedSequence, runSequence);
        }

        [Fact]
        public async Task TestHeartbeatWithLongDelaysAndMediaControl()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture.Receivers[0]);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            outputHelper.WriteLine("Joining device and waiting for 20 seconds...");
            await Task.Delay(20000, Xunit.TestContext.Current.CancellationToken);

            outputHelper.WriteLine("Starting media playback...");
            MediaStatus mediaStatus = await client.MediaChannel.LoadAsync(media);
            Assert.Equal(PlayerStateType.Playing, mediaStatus.PlayerState);

            outputHelper.WriteLine("Media playing, waiting for another 20 seconds...");
            await Task.Delay(20000, Xunit.TestContext.Current.CancellationToken);

            outputHelper.WriteLine("Pausing media...");
            mediaStatus = await client.MediaChannel.PauseAsync();
            Assert.Equal(PlayerStateType.Paused, mediaStatus.PlayerState);

            outputHelper.WriteLine("Test completed successfully - heartbeat maintained through long delays");
            await client.DisconnectAsync();
        }
    }
}
