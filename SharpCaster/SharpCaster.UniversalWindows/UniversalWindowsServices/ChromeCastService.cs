using System.Threading;
using System.Threading.Tasks;
using SharpCaster.Controllers;
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
        public CastButton CastButton { get; set; }
        private CancellationTokenSource _cancellationTokenSource;

        public ChromecastService()
        {
            DeviceLocator = new DeviceLocator();
            DeviceLocator.DeviceFound += DeviceLocator_DeviceFound;
            ChromeCastClient = new ChromeCastClient();
            ChromeCastClient.ConnectedChanged += ChromeCastClient_Connected;

        }

        private void DeviceLocator_DeviceFound(object sender, Chromecast e)
        {
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Disconnected);
        }

        public void ConnectToChromecast(Chromecast chromecast)
        {
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Connecting);
            StopLocatingDevices();
            ConnectedChromecast = chromecast;
            ChromeCastClient.ConnectChromecast(chromecast.DeviceUri);
        }

        private void ChromeCastClient_Connected(object sender, System.EventArgs e)
        {
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Connected);
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