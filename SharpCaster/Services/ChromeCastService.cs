using System.Threading;
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
        private CancellationTokenSource _cancellationTokenSource;

        public ChromecastService()
        {
            DeviceLocator = new DeviceLocator();
            ChromeCastClient = new ChromeCastClient();
        }

  
        public void ConnectToChromecast(Chromecast chromecast)
        {
            StopLocatingDevices();
            ConnectedChromecast = chromecast;
            ChromeCastClient.ConnectChromecast(chromecast.DeviceUri);
        }
        
        public void StopLocatingDevices()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task StartLocatingDevices()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await DeviceLocator.LocateDevicesAsync(_cancellationTokenSource.Token);
        }
    }
}