using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Networking;
using SharpCaster.Models;
using SharpCaster.Interfaces;
using SharpCaster.Services;
using System.Threading;

namespace SharpCaster
{
    public class DeviceLocator
    {
        public TimeSpan TimeOut;
        public ISocketService SocketService;
        public event EventHandler<List<Chromecast>> DeviceListChanged;

        private const string MulticastHostName = "239.255.255.250";
        private const string MulticastPort = "1900";
        private readonly XmlSerializer _xmlSerializer;
        private List<Chromecast> _discoveredDevices;
        private CancellationToken _cancellationToken;
        private List<Uri> _possibleDevicesList; 
        
        public DeviceLocator()
        {
            _xmlSerializer = new XmlSerializer(typeof(DiscoveryRoot));
            SocketService = new SocketService();
            TimeOut = TimeSpan.FromSeconds(4);
            _possibleDevicesList = new List<Uri>();
        }
        
        public async Task<List<Chromecast>> LocateDevicesAsync(CancellationToken token, TimeSpan? delay = null)
        {
            if (delay == null) { delay = TimeSpan.FromMilliseconds(500); }
            _cancellationToken = token;
            var Delay = (TimeSpan)delay;

            _discoveredDevices = new List<Chromecast>();

            var multicastIP = new HostName(MulticastHostName);
            SocketService.MessageReceived += MulticastResponseReceived;
            using (SocketService.Initialize())
            {
                await SocketService.BindEndpointAsync(null, string.Empty);
                SocketService.JoinMulticastGroup(multicastIP);
                do
                {
                    using (var writer = await SocketService.GetOutputWriterAsync(multicastIP, MulticastPort))
                    {
                        const string request = "M-SEARCH * HTTP/1.1\r\n" +
                                               "HOST:" + MulticastHostName + ":" + MulticastPort + "\r\n" +
                                               "ST:SsdpSearch:all\r\n" +
                                               "MAN:\"ssdp:discover\"\r\n" +
                                               "MX:3\r\n\r\n\r\n";
                        writer.WriteString(request);
                        await writer.StoreAsync();
                    }
                    await Task.Delay(Delay);
                }
                while (!_cancellationToken.IsCancellationRequested);
            }
            SocketService.MessageReceived -= MulticastResponseReceived;
            return _discoveredDevices;
        }

        private async void MulticastResponseReceived(object sender, string receivedString)
        {
            if (string.IsNullOrWhiteSpace(receivedString)) return;
            var location = receivedString.Substring(receivedString.ToLower().IndexOf("location:", StringComparison.Ordinal) + 9);
            receivedString = location.Substring(0, location.IndexOf("\r", StringComparison.Ordinal)).Trim();

            Uri uri;
            if (!Uri.TryCreate(receivedString, UriKind.Absolute, out uri)) return;
            if (!uri.AbsoluteUri.EndsWith("/ssdp/device-desc.xml")) return;
            if (_possibleDevicesList.Any(x => x.Equals(uri))) return;
            var possibleChromeCast = new Chromecast { DeviceUri = uri };

            _possibleDevicesList.Add(uri);
            if (!await GetChromecastName(possibleChromeCast)) return;

            _discoveredDevices.Add(possibleChromeCast);
            DeviceListChanged?.Invoke(this, _discoveredDevices);
        }

        private async Task<bool> GetChromecastName(Chromecast chromecast)
        {
            string deviceResponse;
            try {
                deviceResponse = await SocketService.GetStringAsync(chromecast.DeviceUri, TimeOut);
            } catch (Exception ex)
            {
                //TODO: Implement error logging
                return false;
            }
            if (string.IsNullOrWhiteSpace(deviceResponse)) return false;
            var stringReader = new StringReader(deviceResponse);
            var data = (DiscoveryRoot)_xmlSerializer.Deserialize(stringReader);
            if (!data.Device.DeviceType.ToLower().Equals("urn:dial-multiscreen-org:device:dial:1")) return false;
            chromecast.FriendlyName = data.Device.FriendlyName;
            return true;
        }

    }
}