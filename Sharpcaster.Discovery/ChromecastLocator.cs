using Sharpcaster.Core.Interfaces;
using System;
using Sharpcaster.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tmds.MDns;
using System.Linq;

namespace Sharpcaster.Discovery
{
    /// <summary>
    /// Find the available chromecast receivers using mDNS protocol
    /// </summary>
    public class MdnsChromecastLocator : IChromecastLocator
    {
        public event EventHandler<ChromecastReceiver> ChromecastReceivedFound;
        private IList<ChromecastReceiver> DiscoveredDevices { get; set; }
        private ServiceBrowser _serviceBrowser;
        public MdnsChromecastLocator()
        {
            DiscoveredDevices = new List<ChromecastReceiver>();
            _serviceBrowser = new ServiceBrowser();
            _serviceBrowser.ServiceAdded += OnServiceAdded;
        }
        

        private void OnServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            var txtValues = e.Announcement.Txt
                .Select(i => i.Split('='))
                .ToDictionary(y => y[0], y => y[1]);
            if (!txtValues.ContainsKey("fn")) return;
            var ip = e.Announcement.Addresses[0];
            Uri.TryCreate("https://" + ip, UriKind.Absolute, out Uri myUri);
            var chromecast = new ChromecastReceiver
            {
                DeviceUri = myUri,
                Name = txtValues["fn"],
                Model = txtValues["md"],
                Version = txtValues["ve"],
                ExtraInformation = txtValues,
                Status = txtValues["rs"],
                Port = e.Announcement.Port
            };
            ChromecastReceivedFound?.Invoke(this, chromecast);
            DiscoveredDevices.Add(chromecast);
        }
        /// <summary>
        /// Find the available chromecast receivers
        /// </summary>
        public async Task<IEnumerable<ChromecastReceiver>> FindReceiversAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(2000));
            return await FindReceiversAsync(cancellationTokenSource.Token);
        }
        /// <summary>
        /// Find the available chromecast receivers
        /// </summary>
        /// <typeparam name="cancellationToken">Enable to cancel the operation before timeout</typeparam>
        /// <typeparam name="timeOut">Define custom timeout when required, default is 2000 ms</typeparam>
        /// <returns>a collection of chromecast receivers</returns>
        public async Task<IEnumerable<ChromecastReceiver>> FindReceiversAsync(CancellationToken cancellationToken)
        {
            if (_serviceBrowser.IsBrowsing)
            {
                _serviceBrowser.StopBrowse();
            }
            _serviceBrowser.StartBrowse("_googlecast._tcp");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }
            _serviceBrowser.StopBrowse();
            return DiscoveredDevices;
        }
    }
}
