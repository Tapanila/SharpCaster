using Sharpcaster.Models;
using Sharpcaster.Test.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[assembly: AssemblyFixture(typeof(ChromecastDevicesFixture))]
namespace Sharpcaster.Test.helper
{
    // Define a TestFixture to be used by different test classes
    public class ChromecastDevicesFixture : IAsyncLifetime
    {
        // This needs to be static to be used by the MemberData functions of the ChromecastReceiversFilter class allowing to annotate [Theories] with specific list of devices.
        public List<ChromecastReceiver> Receivers = [];
        private int NumberOfSearches = 0;

        public async ValueTask InitializeAsync()
        {
            MdnsChromecastLocator locator = new();
            var receivers = await locator.FindReceiversAsync(TimeSpan.FromMilliseconds(500));
            receivers = receivers.Any() ? receivers : await locator.FindReceiversAsync(TimeSpan.FromSeconds(2));
            receivers = receivers.Any() ? receivers : await locator.FindReceiversAsync(TimeSpan.FromSeconds(5));
            receivers = receivers.Any() ? receivers : await locator.FindReceiversAsync(TimeSpan.FromSeconds(8));
            Receivers = [.. receivers];
            NumberOfSearches++;
        }

        public ValueTask DisposeAsync()
        {
            //Receivers.Clear();
            return ValueTask.CompletedTask;
        }

        public int GetSearchesCnt()
        {
            return NumberOfSearches;
        }
    }

    // Lets use out fixture as 'Collection Fixture' -> its only initilized once for all tests in this collection.
    [CollectionDefinition("SingleCollection")]
    public class ChromecastDevicesFixtureCollection : ICollectionFixture<ChromecastDevicesFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
