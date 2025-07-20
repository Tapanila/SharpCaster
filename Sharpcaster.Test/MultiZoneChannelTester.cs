using Sharpcaster.Models.Media;
using Sharpcaster.Models;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Sharpcaster.Test.helper;

namespace Sharpcaster.Test
{
    public class MultiZoneChannelTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task TestingMultiZone()
        {
            var TestHelper = new TestHelper();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);
            AutoResetEvent _autoResetEvent = new(false);

            //We are going to load video & start playing it
            //Then we are going to change the volume few times

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            int commandsToRun = 5;

            //We are setting up an event listener for multi zone messages
            client.MultiZoneChannel.StatusChanged += (sender, args) =>
            {
                _autoResetEvent.Set();
            };

            //We are setting up an event listener for multi zone messages
            client.MultiZoneChannel.DeviceUpdated += (sender, args) =>
            {
                _autoResetEvent.Set();
            };

            var mediaStatus = await client.MediaChannel.LoadAsync(media);

            for (int i = 0; i < commandsToRun; i++)
            {
                if (i % 2 == 0)
                {
                    await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);
                    await client.ReceiverChannel.SetVolume(0.2);
                }
                else
                {
                    await Task.Delay(1000, Xunit.TestContext.Current.CancellationToken);
                    await client.ReceiverChannel.SetVolume(0.3);
                }
            }

            Assert.True(_autoResetEvent.WaitOne(1000));
        }
    }
}
