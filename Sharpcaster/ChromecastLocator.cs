using Sharpcaster.Interfaces;
using Sharpcaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.MDns;

namespace Sharpcaster
{
    /// <summary>
    /// Find the available chromecast receivers using mDNS protocol
    /// </summary>
    public class MdnsChromecastLocator : IChromecastLocator
    {
        public event EventHandler<ChromecastReceiver> ChromecastReceivedFound;
        private List<ChromecastReceiver> DiscoveredDevices { get; }
        private readonly ServiceBrowser _serviceBrowser;
        private SemaphoreSlim ServiceAddedSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        private static readonly string[] stringArray = new string[] { "", "" };

        public MdnsChromecastLocator()
        {
            DiscoveredDevices = new List<ChromecastReceiver>();
            _serviceBrowser = new ServiceBrowser();
            _serviceBrowser.ServiceAdded += OnServiceAdded;
        }

        private void OnServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            ServiceAddedSemaphoreSlim.Wait();
            try
            {
                var txtValuess = e.Announcement.Txt;
                if (txtValuess == null)
                {
                    return;
                }

                var txtValues = e.Announcement.Txt
                    .Select(i =>
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            return i.Split('=');
                        }
                        return stringArray;
                    })
                    .ToDictionary(y => y[0], y => y[1]);
                if (!txtValues.TryGetValue("fn", out string value)) return;
                var ip = e.Announcement.Addresses[0];
                Uri.TryCreate("https://" + ip, UriKind.Absolute, out Uri myUri);
                var chromecast = new ChromecastReceiver
                {
                    DeviceUri = myUri,
                    Name = value,
                    Model = txtValues["md"],
                    Version = txtValues["ve"],
                    ExtraInformation = txtValues,
                    Status = txtValues["rs"],
                    Port = e.Announcement.Port
                };
                ChromecastReceivedFound?.Invoke(this, chromecast);
                DiscoveredDevices.Add(chromecast);
            }
            finally
            {
                ServiceAddedSemaphoreSlim.Release();
            }
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
        /// <param name="cancellationToken">Enable to cancel the operation</param>
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
                await Task.Delay(100, cancellationToken);
            }
            _serviceBrowser.StopBrowse();
            return DiscoveredDevices;
        }
    }
}
