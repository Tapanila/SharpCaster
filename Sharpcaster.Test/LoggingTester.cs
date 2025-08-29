using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class LoggingTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public void TestLogging()
        {
            var TestHelper = new TestHelper();
            List<string> logLines = [];
            _ = TestHelper.GetClientWithTestOutput(outputHelper, assertableLog: logLines);

            Assert.Equal("MessageTypes: [addUserResponse,getInfoResponse,LAUNCH_ERROR,RECEIVER_STATUS,QUEUE_CHANGE,QUEUE_ITEM_IDS,QUEUE_ITEMS,DEVICE_UPDATED,MULTIZONE_STATUS,ERROR,INVALID_REQUEST,LOAD_CANCELLED,LOAD_FAILED,MEDIA_STATUS,PING,CLOSE,LAUNCH_STATUS]", logLines[0]);
        }

        [Fact]
        public async Task TestPlayMediaWorksWithoutLogging()
        {
            ChromecastClient client = new();
            await client.ConnectChromecast(fixture.Receivers[0]);
            await client.LaunchApplicationAsync("B3419EF5", false);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.MediaChannel.LoadAsync(media);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Single(status.Items);
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);
        }
    }
}
