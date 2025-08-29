using Sharpcaster.Models;
using Sharpcaster.Test.helper;
using System.Threading.Tasks;
using Xunit;
using Sharpcaster.Models.Media;
using System.Threading;

namespace Sharpcaster.Test
{
    public class MultipleClientTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
    {
        [Fact]
        public async Task TestTwoClients()
        {
            var TestHelper = new TestHelper();
            AutoResetEvent _autoResetEvent = new(false);
            var client1 = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);
            var client2 = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            await client2.LaunchApplicationAsync("B3419EF5", true);

            client2.MediaChannel.StatusChanged += (sender, e) =>
            {
                _autoResetEvent.Set();
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            var mediaStatus = await client1.MediaChannel.LoadAsync(media);
            await client1.MediaChannel.PlayAsync();

            Assert.True(_autoResetEvent.WaitOne(3000));
        }

        [Fact]
        public async Task TestThreeClients()
        {
            var TestHelper = new TestHelper();
            AutoResetEvent _autoResetEvent2 = new(false);
            AutoResetEvent _autoResetEvent3 = new(false);
            var client1 = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);
            var client2 = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            var client3 = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            await client2.LaunchApplicationAsync("B3419EF5", true);
            await client3.LaunchApplicationAsync("B3419EF5", true);

            client2.MediaChannel.StatusChanged += (sender, e) =>
            {
                _autoResetEvent2.Set();
            };

            client3.MediaChannel.StatusChanged += (sender, e) =>
            {
                _autoResetEvent3.Set();
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            var mediaStatus = await client1.MediaChannel.LoadAsync(media);
            await client1.MediaChannel.PlayAsync();

            Assert.True(_autoResetEvent2.WaitOne(3000));
            Assert.True(_autoResetEvent3.WaitOne(100));
        }

        [Fact]
        public async Task TestCommandsFromMultipleDifferentClients()
        {
            var TestHelper = new TestHelper();
            var client1 = await TestHelper.CreateConnectAndLoadAppClient(outputHelper, fixture);
            var client2 = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            var client3 = await TestHelper.CreateAndConnectClient(outputHelper, fixture);
            await client2.LaunchApplicationAsync("B3419EF5", true);
            await client3.LaunchApplicationAsync("B3419EF5", true);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            await client1.MediaChannel.LoadAsync(media);
            await client1.MediaChannel.PlayAsync();
            await client2.MediaChannel.PauseAsync();
            await client3.MediaChannel.PlayAsync();

            Assert.NotEqual(client1.SenderId, client2.SenderId);
            Assert.NotEqual(client1.SenderId, client3.SenderId);
            Assert.NotEqual(client2.SenderId, client3.SenderId);
        }
    }
}
