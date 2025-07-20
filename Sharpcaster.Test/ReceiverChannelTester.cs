using Sharpcaster.Models;
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
            var client = await TestHelper.CreateAndConnectClient(outputHelper, fixture);

            var status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);

            await Task.Delay(500, Xunit.TestContext.Current.CancellationToken);      // My Chromecast Audio device (somtimes) needs this delay here to pass the test, because the first volume requests triggers a lot of responses 
                                        // (some of them on a 'multizone' channel) with eualizer data !? and the 2nd request does not get the new volume but another answer with old 0.1 as volume !!!
                                        // It happens always if this test runs directly after the TestStoppingApplication!?

            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
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
