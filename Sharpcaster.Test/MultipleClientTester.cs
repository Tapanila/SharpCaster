using Sharpcaster.Channels;
using Sharpcaster.Models;
using Sharpcaster.Test.helper;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using System.Threading;
using System.Linq;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class MultipleClientTester : IClassFixture<ChromecastDevicesFixture>
    {
        private ITestOutputHelper output;

        public MultipleClientTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }


        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetChromecastUltra), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestTwoClients(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
            var client1 = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);
            var client2 = await TestHelper.CreateAndConnectClient(output, receiver);
            await client2.LaunchApplicationAsync("B3419EF5", true);

            client2.GetChannel<IMediaChannel>().StatusChanged += (sender, e) =>
            {
                _autoResetEvent.Set();
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };

            
            var mediaStatus = await client1.GetChannel<IMediaChannel>().LoadAsync(media);
            await client1.GetChannel<IMediaChannel>().PlayAsync();

            Assert.True(_autoResetEvent.WaitOne(3000));
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetChromecastUltra), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestThreeClients(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            AutoResetEvent _autoResetEvent2 = new AutoResetEvent(false);
            AutoResetEvent _autoResetEvent3 = new AutoResetEvent(false);
            var client1 = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);
            var client2 = await TestHelper.CreateAndConnectClient(output, receiver);
            var client3 = await TestHelper.CreateAndConnectClient(output, receiver);
            await client2.LaunchApplicationAsync("B3419EF5", true);
            await client3.LaunchApplicationAsync("B3419EF5", true);

            client2.GetChannel<IMediaChannel>().StatusChanged += (sender, e) =>
            {
                _autoResetEvent2.Set();
            };

            client3.GetChannel<IMediaChannel>().StatusChanged += (sender, e) =>
            {
                _autoResetEvent3.Set();
            };

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };


            var mediaStatus = await client1.GetChannel<IMediaChannel>().LoadAsync(media);
            await client1.GetChannel<IMediaChannel>().PlayAsync();

            Assert.True(_autoResetEvent2.WaitOne(3000));
            Assert.True(_autoResetEvent3.WaitOne(100));
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetChromecastUltra), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestCommandsFromMultipleDifferentClients(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            AutoResetEvent _autoResetEvent2 = new AutoResetEvent(false);
            AutoResetEvent _autoResetEvent3 = new AutoResetEvent(false);
            var client1 = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);
            var client2 = await TestHelper.CreateAndConnectClient(output, receiver);
            var client3 = await TestHelper.CreateAndConnectClient(output, receiver);
            await client2.LaunchApplicationAsync("B3419EF5", true);
            await client3.LaunchApplicationAsync("B3419EF5", true);

            var media = new Media
            {
                ContentUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4"
            };


            var mediaStatus = await client1.GetChannel<IMediaChannel>().LoadAsync(media);
            await client1.GetChannel<IMediaChannel>().PlayAsync();

            await client2.GetChannel<IMediaChannel>().PauseAsync();

            await client3.GetChannel<IMediaChannel>().PlayAsync();

            Assert.NotEqual(client1.SenderId, client2.SenderId);
            Assert.NotEqual(client1.SenderId, client3.SenderId);
            Assert.NotEqual(client2.SenderId, client3.SenderId);
        }
    }
}
