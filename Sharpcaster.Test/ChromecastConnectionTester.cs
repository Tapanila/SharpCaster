using Microsoft.VisualStudio.TestPlatform.Utilities;
using Sharpcaster.Models;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Sharpcaster.Test.helper;

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
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        //[MemberData(nameof(CCDevices.GetJblSpeaker), MemberType = typeof(CCDevices))]
        //[MemberData(nameof(CCDevices.GetAny), MemberType = typeof(CCDevices))]
        public async Task SearchChromecastsAndConnectToIt(ChromecastReceiver receiver)
        {
            var status = await TestHelper.CreateAndConnectClient(output, receiver);
            Assert.NotNull(status);
        }

        [Theory(Skip ="Test needs manuell interactions -> skipped for autotestings")]
        //[MemberData(nameof(CCDevices.GetAny), MemberType = typeof(CCDevices))]
        [MemberData(nameof(ChromecastReceiversFilter.GetJblSpeaker), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task SearchChromecastsAndConnectToItThenWaitForItToShutdown(ChromecastReceiver receiver)
        {
            var client = await TestHelper.CreateConnectAndLoadAppClient(output, receiver);
            
            Assert.NotNull(client.GetChromecastStatus());
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

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

    }
}
