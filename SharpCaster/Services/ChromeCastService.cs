using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Services
{
    public class ChromecastService
    {
        private static ChromecastService _instance;

        public static ChromecastService Current => _instance ?? (_instance = new ChromecastService());

        public DeviceLocator DeviceLocator { get; }
        public ChromeCastClient ChromeCastClient { get; }
        public Chromecast ConnectedChromecast { get; set; }

        public ChromecastService()
        {
            DeviceLocator = new DeviceLocator();
            ChromeCastClient = new ChromeCastClient();
        }

  
        public void ConnectToChromecast(Chromecast chromecast)
        {
            ConnectedChromecast = chromecast;
            ChromeCastClient.ConnectChromecast(chromecast.DeviceUri);
        }
        

        public async Task<ObservableCollection<Chromecast>> StartLocatingDevices()
        {
            return await DeviceLocator.LocateDevicesAsync();
        }
    }
}