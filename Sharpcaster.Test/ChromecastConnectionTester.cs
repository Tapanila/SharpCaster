using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ChromecastConnectionTester
    {
        [Fact]
        public async Task SearchChromecastsAndConnectToIt()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            Assert.NotNull(status);
        }

    }
}
