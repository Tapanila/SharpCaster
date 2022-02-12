using Sharpcaster.Core.Interfaces;
using Sharpcaster.Core.Models;
using System;
using System.Threading;
using Xunit;

namespace Sharpcaster.Test
{
    public class MdnsChromecastLocatorTester
    {
        [Fact]
        public async void SearchChromecasts()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);
        }

        [Fact]
        public async void SearchChromecastsTrickerEvent()
        {
            int counter = 0;
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            locator.ChromecastReceivedFound += delegate (object sender, ChromecastReceiver e)
            {
                counter++;
            };
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);
            Assert.NotEqual(counter, 0);
        }

        [Fact]
        public async void SearchChromecastsWithTooShortTimeout()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(0));
            var chromecasts = await locator.FindReceiversAsync(cancellationTokenSource.Token);
            Assert.Empty(chromecasts);
        }
        

        [Fact]
        public async void SearchChromecastsCancellationToken()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
            var chromecasts = await locator.FindReceiversAsync(source.Token);
            Assert.NotEmpty(chromecasts);
        }
    }
}
