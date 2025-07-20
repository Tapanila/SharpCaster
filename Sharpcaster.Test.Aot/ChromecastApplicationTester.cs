namespace Sharpcaster.Test.Aot
{
    [TestClass]
    public class ChromecastApplicationTester
    {
        [TestMethod]
        public async Task ConnectLaunchDisconnect()
        {
            // Test 1: Discover at least one Chromecast device
            var locator = new MdnsChromecastLocator();
            var receivers = await locator.FindReceiversAsync();
            if (receivers?.Any() != true)
            {
                throw new Exception("No Chromecast devices found on the network.");
            }

            // Test 2: Connect to the first Chromecast and launch an app
            var receiver = receivers.First();
            var client = new ChromecastClient();
            await client.ConnectChromecast(receiver);
            var status = await client.LaunchApplicationAsync("B3419EF5");
            if (status.Application?.AppId != "B3419EF5")
            {
                throw new Exception("Failed to launch application on Chromecast.");
            }

            // Test 3: Disconnect
            await client.DisconnectAsync();
        }
    }
}
