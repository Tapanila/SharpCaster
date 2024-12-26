using Sharpcaster.Models;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Sharpcaster.Test.helper;
using Sharpcaster.Models.Media;
using System.Linq;
using System.Collections.Generic;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ChromecastConnectionTester : IClassFixture<ChromecastDevicesFixture>
    {
        private ITestOutputHelper output;

        public ChromecastConnectionTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task SearchChromecastsAndConnectToIt(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var status = await TestHelper.CreateAndConnectClient(output, receiver);
            Assert.NotNull(status);
        }

        [Theory(Skip = "Test needs manuell interactions -> skipped for autotestings")]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task SearchChromecastsAndConnectToItThenWaitForItToShutdown(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);

            Assert.NotNull(client.GetChromecastStatus());
            AutoResetEvent _autoResetEvent = new(false);

            client.Disconnected += (sender, args) =>
            {
                output.WriteLine("Chromecast did shutdown");
                _autoResetEvent.Set();
            };

            //This checks that within 30 seconds we have noticed that device was turned off
            //This need manual intervention to turn off the device
            output.WriteLine("Waiting for Chromecast to shutdown");
            Assert.True(_autoResetEvent.WaitOne(30000), "This test fails if run without manunal intervention on the used cast devide!");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestingHeartBeat(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);
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
                    await Task.Delay(1000);
                    mediaStatus = await client.MediaChannel.PauseAsync();
                    Assert.Equal(PlayerStateType.Paused, mediaStatus.PlayerState);
                    runSequence += pause;
                }
                else
                {
                    await Task.Delay(1000);
                    mediaStatus = await client.MediaChannel.PlayAsync();
                    Assert.Equal(PlayerStateType.Playing, mediaStatus.PlayerState);
                    runSequence += play;
                }
            }

            string expectedSequence = "Load" + string.Concat(Enumerable.Repeat(pause + play, commandsToRun / 2));

            Assert.Equal(expectedSequence, runSequence);
        }
    }
}
