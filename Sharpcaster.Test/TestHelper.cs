using Sharpcaster.Core.Interfaces;
using Sharpcaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sharpcaster.Test
{
    public static class TestHelper
    {
        public async static Task<ChromecastReceiver> FindChromecast()
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            return chromecasts.First();
        }

        public async static Task<ChromecastReceiver> FindChromecast(string name, double timeoutSeconds)
        {
            IChromecastLocator locator = new Discovery.MdnsChromecastLocator();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
            var chromecasts = await locator.FindReceiversAsync(cts.Token);
            return chromecasts.First(x => x.Name == name);
        }
    }
}
