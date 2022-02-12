using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastConnectionTester
    {
        [Fact]
        public async void SearchChromecastsAndConnectToIt()
        {
            var chromecast = await TestHelper.FindChromecast();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            Assert.NotNull(status);
        }

    }
}
