using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Test.helper;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    public class ReceiverChannelTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task TestMute()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);

            var status = await client.ReceiverChannel.SetMute(true);

            Assert.True(status.Volume.Muted);
        }

        [Fact]
        public async Task TestUnMute()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);

            var status = await client.ReceiverChannel.SetMute(false);
            Assert.False(status.Volume.Muted);
        }

        [Fact]
        public async Task TestVolume()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);
            
            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4",
                ContentType = "video/mp4",
                Metadata = new MediaMetadata
                {
                    Title = "Designing for Google Cast",
                    Images = new[]
                    {
                        new Image
                        {
                            Url = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/poster/DesigningForGoogleCast.jpg"
                        }
                    }
                },
            };

            await client.MediaChannel.LoadAsync(media);

            var status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.5);
            Assert.Equal(0.5, status.Volume.Level.Value, precision: 1);
            status = await client.ReceiverChannel.SetVolume(0.8);
            Assert.Equal(0.8, status.Volume.Level.Value, precision: 1);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task TestStoppingApplication()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);

            await client.LaunchApplicationAsync("B3419EF5");

            var status = await client.ReceiverChannel.StopApplication();
            Assert.Null(status.Applications);
        }

        [Fact]
        public async Task TestApplicationLaunchStatusMessage()
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);

            string launchStatus = "";

            client.ReceiverChannel.LaunchStatusChanged += (sender, e) =>
            {
                launchStatus = e.Status;
            };

            await client.LaunchApplicationAsync("B3419EF5");

            var status = await client.ReceiverChannel.StopApplication();
            Assert.Null(status.Applications);
        }
    }
}
