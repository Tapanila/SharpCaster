using Sharpcaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastConnectionTester
    {
        [Fact]
        public async void SearchChromecastsAndConnectToIt()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);

            var chromecast = chromecasts.First();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);
            Assert.NotNull(status);
        }

    }
}
