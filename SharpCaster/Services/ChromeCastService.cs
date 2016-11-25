using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Services
{
    public class ChromecastService
    {
        private static readonly Lazy<ChromecastService> _current = new Lazy<ChromecastService>(() => new ChromecastService());
        public static ChromecastService Current => _current.Value;

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