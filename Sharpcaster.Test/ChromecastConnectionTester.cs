using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ChromecastConnectionTester
    {

        private ITestOutputHelper output;
        public ChromecastConnectionTester(ITestOutputHelper outputHelper)
        {
            output = outputHelper;
        }

        [Fact]
        public async Task SearchChromecastsAndConnectToIt()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            Assert.NotNull(status);
        }

        [Fact]
        public async Task SearchChromecastsAndConnectToItThenWaitForItToShutdown()
        {
            var client = await TestHelper.CreateConnectAndLoadAppClient(output);
            
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
            Assert.True(_autoResetEvent.WaitOne(30000));
        }

    }
}
