using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class LoggingTester(ITestOutputHelper outputHelper)
    {
        readonly ITestOutputHelper output = outputHelper;

        [Fact]
        public void TestLogging()
        {
            var TestHelper = new TestHelper();
            List<string> logLines = [];
            _ = TestHelper.GetClientWithTestOutput(output, assertableLog: logLines);

            Assert.Equal("[addUserResponse,getInfoResponse,LAUNCH_ERROR,RECEIVER_STATUS,QUEUE_CHANGE,QUEUE_ITEM_IDS,QUEUE_ITEMS,DEVICE_UPDATED,MULTIZONE_STATUS,ERROR,INVALID_REQUEST,LOAD_CANCELLED,LOAD_FAILED,MEDIA_STATUS,PING,CLOSE]", logLines[0]);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestPlayMediaWorksWithoutLogging(ChromecastReceiver receiver)
        {
            ChromecastClient client = new();
            await client.ConnectChromecast(receiver);
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
