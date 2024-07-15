using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class LoggingTester
    {
        ITestOutputHelper output;
        public LoggingTester(ITestOutputHelper outputHelper) {
            output = outputHelper;
        }

        [Fact]
        public void TestLogging() {
            var TestHelper = new TestHelper();
            List<string> logLines = new List<string>();
            var client = TestHelper.GetClientWithTestOutput(output, assertableLog: logLines);

            //var loggerFactory = TestHelper.CreateMockedLoggerFactory(logLines);

            //var client = new ChromecastClient(loggerFactory: loggerFactory);
            Assert.Equal("[LAUNCH_ERROR,RECEIVER_STATUS,QUEUE_CHANGE,QUEUE_ITEM_IDS,QUEUE_ITEMS,DEVICE_UPDATED,MULTIZONE_STATUS,INVALID_REQUEST,LOAD_CANCELLED,LOAD_FAILED,MEDIA_STATUS,PING,CLOSE]", logLines[0]);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestPlayMediaWorksWithoutLogging(ChromecastReceiver receiver) {
            ChromecastClient client = new ChromecastClient();
            await client.ConnectChromecast(receiver);
            await client.LaunchApplicationAsync("B3419EF5", false);
            
            var media = new Media {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            MediaStatus status = await client.GetChannel<IMediaChannel>().LoadAsync(media);

            Assert.Equal(PlayerStateType.Playing, status.PlayerState);
            Assert.Single(status.Items);
            Assert.Equal(status.CurrentItemId, status.Items[0].ItemId);

        }


    }
}
