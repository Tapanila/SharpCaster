using Sharpcaster.Interfaces;
using Sharpcaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test.helper {

    // Define a TestFixture to be used by different test classes
    public class ChromecastDevicesFixture : IDisposable {

        // This needs to be static to be used by the MemberData functions of the ChromecastReceiversFilter class allowing to annotate [Theories] with specific list of devices.
        public static List<ChromecastReceiver> Receivers = new List<ChromecastReceiver>();
        public static int NumberOfSearches = 0;

        static ChromecastDevicesFixture() {
            IChromecastLocator locator = new MdnsChromecastLocator();
            var t = locator.FindReceiversAsync();
            t.Wait();
            Receivers = t.Result.ToList();
            NumberOfSearches++;
        }

        public int GetSearchesCnt() {
            return NumberOfSearches;
        }

        public void Dispose() {
            //Receivers.Clear();
        }

    }


    // Lets use out fixture as 'Collection Fixture' -> its only initilized once for all tests in this collection.
    [CollectionDefinition("SingleCollection")]
    public class ChromecastDevicesFixtureCollection : ICollectionFixture<ChromecastDevicesFixture> {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}
