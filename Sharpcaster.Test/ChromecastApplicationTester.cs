using Sharpcaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sharpcaster.Test
{
    public class ChromecastApplicationTester
    {
        [Fact]
        public async void ConnectToChromecastAndLaunchApplication()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);

            var chromecast = chromecasts.First();
            var client = new ChromecastClient();
            var status = await client.ConnectChromecast(chromecast);

            status = await client.LaunchApplicationAsync("B3419EF5");
            //We make sure that the application was launched succesfully
            Assert.Equal(status.Applications[0].AppId, "B3419EF5");
        }
    }
}
