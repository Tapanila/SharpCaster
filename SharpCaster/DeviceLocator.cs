using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rssdp;
using Rssdp.Infrastructure;
using SharpCaster.Annotations;
using SharpCaster.Models;

namespace SharpCaster
{
    public class DeviceLocator : INotifyPropertyChanged
    {
        public ObservableCollection<Chromecast> DiscoveredDevices { get; set; }

        public DeviceLocator()
        {
            DiscoveredDevices = new ObservableCollection<Chromecast>();
        }

        public async Task<ObservableCollection<Chromecast>> LocateDevicesAsync()
        {
            return await LocateDevicesAsync(new SsdpDeviceLocator());
        }

        public async Task<ObservableCollection<Chromecast>> LocateDevicesAsync(string localIpAdress)
        {
            return await LocateDevicesAsync(new SsdpDeviceLocator(new SsdpCommunicationsServer(new SocketFactory(localIpAdress))));
        }

        private async Task<ObservableCollection<Chromecast>> LocateDevicesAsync(SsdpDeviceLocator deviceLocator)
        {
            using (deviceLocator)
            {
                var deviceType = "urn:dial-multiscreen-org:device:dial:1";
                deviceLocator.NotificationFilter = deviceType;
                var foundDevices = await deviceLocator.SearchAsync(deviceType, TimeSpan.FromMilliseconds(5000));
                
                foreach (var foundDevice in foundDevices)
                {
                    var fullDevice = await foundDevice.GetDeviceInfo();
                    Uri myUri;
                    Uri.TryCreate("https://" + foundDevice.DescriptionLocation.Host, UriKind.Absolute, out myUri);
                    var chromecast = new Chromecast
                    {
                        DeviceUri = myUri,
                        FriendlyName = fullDevice.FriendlyName
                    };
                    DiscoveredDevices.Add(chromecast);
                }
            }
            return DiscoveredDevices;
        }




        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}