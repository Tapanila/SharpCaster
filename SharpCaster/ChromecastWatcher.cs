using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rssdp;
using Rssdp.Infrastructure;
using SharpCaster.Annotations;
using SharpCaster.Models;

namespace SharpCaster
{
    public class ChromecastWatcher
    {
        private const string SsdpFilter = "urn:dial-multiscreen-org:device:dial:1";
        private SsdpDeviceLocator _deviceLocator;
        private readonly string _localIpAddress;

        /// <remarks>
        /// Only used for LocateDevicesAsync
        /// </remarks>
        public ObservableCollection<ChromecastInformation> DiscoveredDevices { get; set; }

        public event EventHandler<ChromecastInformation> Added;
        public event EventHandler<ChromecastInformation> Removed;
        public event EventHandler<ChromecastInformation> Updated;

        //TODO not sure how this should be used
        //public event EventHandler EnumerationCompleted;

        public event EventHandler Stopped;

        public ChromecastWatcherStatus Status { get; private set; }

        public ChromecastWatcher() : this("")
        {
        }

        public ChromecastWatcher(string localIpAddress)
        {
            _localIpAddress = localIpAddress;

            SetupDeviceLocator();

            Status = ChromecastWatcherStatus.Created;
            DiscoveredDevices = new ObservableCollection<ChromecastInformation>();
        }

        private void SetupDeviceLocator()
        {
            _deviceLocator = GetDeviceLocatorForIp(_localIpAddress);
            _deviceLocator.NotificationFilter = SsdpFilter;
        }

        private SsdpDeviceLocator GetDeviceLocatorForIp(string localIpAddress)
        {
            return string.IsNullOrWhiteSpace(localIpAddress)
                ? new SsdpDeviceLocator() //new SsdpCommunicationsServer(new SocketFactory(null)))
                : new SsdpDeviceLocator(new SsdpCommunicationsServer(new SocketFactory(localIpAddress)));
        }

        public void Start()
        {
            if (Status != ChromecastWatcherStatus.Created
                && Status != ChromecastWatcherStatus.Stopped
                && Status != ChromecastWatcherStatus.Aborted)
            {
                Debug.WriteLine($"Cannot start watching while status is {Status}");
                return;
            }

            Status = ChromecastWatcherStatus.Started;
            try
            {
                _deviceLocator.DeviceAvailable += _deviceLocator_DeviceAvailable;
                _deviceLocator.DeviceUnavailable += _deviceLocator_DeviceUnavailable;
                _deviceLocator.StartListeningForNotifications();
                _deviceLocator.SearchAsync();
            }
            catch (ObjectDisposedException ex)
            {
                Status = ChromecastWatcherStatus.Aborted;
                _deviceLocator.Dispose();
                SetupDeviceLocator();
            }
        }

        public void Stop()
        {
            Status = ChromecastWatcherStatus.Stopping;

            try
            {
                _deviceLocator.DeviceAvailable -= _deviceLocator_DeviceAvailable;
                _deviceLocator.DeviceUnavailable -= _deviceLocator_DeviceUnavailable;
                _deviceLocator.StopListeningForNotifications();
                Status = ChromecastWatcherStatus.Stopped;
            }
            catch (ObjectDisposedException ex)
            {
                Status = ChromecastWatcherStatus.Aborted;
                _deviceLocator.Dispose();
                SetupDeviceLocator();
            }
        }

        private async void _deviceLocator_DeviceUnavailable(object sender, DeviceUnavailableEventArgs e)
        {
            var chromecastInfomation = await ConvertToChromecastInformation(e.DiscoveredDevice);
            Removed?.Invoke(this, chromecastInfomation);
        }

        private async void _deviceLocator_DeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            var chromecastInformation = await ConvertToChromecastInformation(e.DiscoveredDevice);
            if (e.IsNewlyDiscovered)
            {
                Added?.Invoke(this, chromecastInformation);
            }
            else
            {
                Updated?.Invoke(this, chromecastInformation);
            }
        }

        
        
        public async Task<ObservableCollection<ChromecastInformation>> FindDevicesAsync()
        {
            using (var deviceLocator = GetDeviceLocatorForIp(_localIpAddress))
            {
                var foundDevices = await deviceLocator.SearchAsync(SsdpFilter, TimeSpan.FromMilliseconds(5000));

                foreach (var foundDevice in foundDevices)
                {
                    ChromecastInformation chromecast = await ConvertToChromecastInformation(foundDevice);
                    DiscoveredDevices.Add(chromecast);
                }
            }
            return DiscoveredDevices;
        }

        private static async Task<ChromecastInformation> ConvertToChromecastInformation(DiscoveredSsdpDevice foundDevice)
        {
            var fullDevice = await foundDevice.GetDeviceInfo();
            Uri myUri;
            Uri.TryCreate("https://" + foundDevice.DescriptionLocation.Host, UriKind.Absolute, out myUri);
            var chromecast = new ChromecastInformation
            {
                DeviceUri = myUri,
                FriendlyName = fullDevice.FriendlyName,
                SsdpUuid = Guid.Parse(fullDevice.Uuid)
            };
            return chromecast;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ChromecastWatcherStatus
    {
        Aborted,
        Created,

        //TODO not sure how this should be used
        //EnumerationCompleted,
        Started,
        Stopped,
        Stopping
    }
}