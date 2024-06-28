using Sharpcaster.Interfaces;
using Sharpcaster.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class MdnsChromecastLocatorTester
    {
        [Fact]
        public async Task SearchChromecasts()
        {
            IChromecastLocator locator = new MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);
        }

        [Fact]
        public async Task SearchChromecastsTrickerEvent()
        {
            int counter = 0;
            IChromecastLocator locator = new MdnsChromecastLocator();
            locator.ChromecastReceivedFound += delegate (object sender, ChromecastReceiver e)
            {
                counter++;
            };
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);
            Assert.NotEqual(0, counter);
        }

        [Fact]
        public async Task SearchChromecastsWithTooShortTimeout()
        {
            IChromecastLocator locator = new MdnsChromecastLocator();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(0));
            var chromecasts = await locator.FindReceiversAsync(cancellationTokenSource.Token);
            Assert.Empty(chromecasts);
        }


        [Fact]
        public async Task SearchChromecastsCancellationToken()
        {
            IChromecastLocator locator = new MdnsChromecastLocator();
            var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
            var chromecasts = await locator.FindReceiversAsync(source.Token);
            Assert.NotEmpty(chromecasts);
        }
    }
}
