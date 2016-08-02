using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SharpCaster.Models;
using SharpCaster.Interfaces;
using SharpCaster.Services;
using System.Threading;
using SharpCaster.Annotations;

namespace SharpCaster
{
    public class DeviceLocator : INotifyPropertyChanged
    {
        public TimeSpan TimeOut;
        public ISocketService SocketService;
        public event EventHandler<Chromecast> DeviceFound;

        public List<Chromecast> DiscoveredDevices
        {
            get { return _discoveredDevices; }
            private set { _discoveredDevices = value; OnPropertyChanged(); }
        }

        private List<Chromecast> _discoveredDevices; 

        private const string MulticastHostName = "239.255.255.250";
        private const string MulticastPort = "1900";
        private readonly XmlSerializer _xmlSerializer;
        private CancellationToken _cancellationToken;
        private List<Uri> _possibleDevicesList; 
        
        public DeviceLocator()
        {
            _xmlSerializer = new XmlSerializer(typeof(DiscoveryRoot));
            SocketService = new SocketService();
            TimeOut = TimeSpan.FromSeconds(4);
            _possibleDevicesList = new List<Uri>();
        }

        public async Task<List<Chromecast>> LocateDevicesAsync(CancellationToken token)
        {
            return await LocateDevicesAsync(token, TimeSpan.FromMilliseconds(500));
        }

        public async Task<List<Chromecast>> LocateDevicesAsync(CancellationToken token, TimeSpan delay)
        {
            _cancellationToken = token;
            
            DiscoveredDevices = new List<Chromecast>();

            SocketService.MessageReceived += MulticastResponseReceived;
            using (SocketService.Initialize())
            {
                try
                {
                    await SocketService.BindEndpointAsync(null, string.Empty);
                    SocketService.JoinMulticastGroup(MulticastHostName);
                    do
                    {
                        const string request = "M-SEARCH * HTTP/1.1\r\n" +
                                                   "HOST:" + MulticastHostName + ":" + MulticastPort + "\r\n" +
                                                   "ST:SsdpSearch:all\r\n" +
                                                   "MAN:\"ssdp:discover\"\r\n" +
                                                   "MX:3\r\n\r\n\r\n";

                        await SocketService.Write(request, MulticastPort, MulticastHostName);
                        
                        await Task.Delay(delay, token);
                    } while (!_cancellationToken.IsCancellationRequested);
                }
                catch (Exception ex)
                {
                    return DiscoveredDevices;
                }
                finally
                {
                    SocketService.MessageReceived -= MulticastResponseReceived;
                }
            }
            return DiscoveredDevices;
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

            DiscoveredDevices.Add(possibleChromeCast);
            DeviceFound?.Invoke(this, possibleChromeCast);
        }

        private async Task<bool> GetChromecastName(Chromecast chromecast)
        {
            string deviceResponse;
            try {
                deviceResponse = await SocketService.GetStringAsync(chromecast.DeviceUri, TimeOut);
            } catch (Exception ex)
            {
                _possibleDevicesList.Remove(chromecast.DeviceUri);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}