using Sharpcaster.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    /// <summary>
    /// Interface for the device locator
    /// </summary>
    public interface IChromecastLocator
    {
        /// <summary>
        /// Find the available chromecast receivers
        /// </summary>
        /// <typeparam name="cancellationToken">Enable to change the operation default timeout which is 2000 ms</typeparam>
        /// <returns>a collection of chromecast receivers</returns>
        Task<IEnumerable<ChromecastReceiver>> FindReceiversAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Find the available chromecast receivers. Default timeout is 2000 ms
        /// </summary>
        /// <returns>a collection of chromecast receivers</returns>
        Task<IEnumerable<ChromecastReceiver>> FindReceiversAsync();
        /// <summary>
        /// Fires when chromecastreceiver is found. You can use this with the combination of FindReceiverAsync in special cases.
        /// </summary>
        /// <returns>single chromecast receiver</returns>

        event EventHandler<ChromecastReceiver> ChromecastReceivedFound;
    }
}
