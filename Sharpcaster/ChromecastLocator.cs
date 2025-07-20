using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sharpcaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace Sharpcaster
{
    /// <summary>
    /// Event arguments for when a Chromecast receiver is found
    /// </summary>
    public class ChromecastReceiverEventArgs : EventArgs
    {

        /// <summary>
        /// Gets the discovered Chromecast receiver
        /// </summary>
        public ChromecastReceiver Receiver { get; }

        /// <summary>
        /// Initializes a new instance of the ChromecastReceiverEventArgs class
        /// </summary>
        /// <param name="receiver">The discovered Chromecast receiver</param>
        public ChromecastReceiverEventArgs(ChromecastReceiver receiver)
        {
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }
    }

    /// <summary>
    /// Find the available chromecast receivers using mDNS protocol
    /// </summary>
    public class MdnsChromecastLocator : IDisposable
    {
        /// <summary>
        /// Occurs when a new Chromecast receiver is found during continuous discovery
        /// </summary>
        public event EventHandler<ChromecastReceiverEventArgs>? ChromecastReceiverFound;
        private readonly ILogger<MdnsChromecastLocator> _logger;
        private CancellationTokenSource? _continuousSearchCts;

        /// <summary>
        /// Creates a new instance of MdnsChromecastLocator with a logger
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public MdnsChromecastLocator(ILogger<MdnsChromecastLocator>? logger = null)
        {
            _logger = logger ?? NullLogger<MdnsChromecastLocator>.Instance;
        }

        /// <summary>
        /// Find the available chromecast receivers (async, no events)
        /// </summary>
        /// <param name="timeout">Discovery timeout (default 2 seconds)</param>
        /// <returns>Collection of chromecast receivers</returns>
        public async Task<IEnumerable<ChromecastReceiver>> FindReceiversAsync(TimeSpan? timeout = null)
        {
            var discoveryTimeout = timeout ?? TimeSpan.FromSeconds(2);
            var devices = new List<ChromecastReceiver>();
            
            _mdnsDiscoveryStarted(_logger, discoveryTimeout.TotalMilliseconds, null);
            
            try
            {
                var responses = await ZeroconfResolver.ResolveAsync(
                    "_googlecast._tcp.local.", 
                    discoveryTimeout).ConfigureAwait(false);
                
                var responsesList = responses.ToList();
                _mdnsDiscoveryResponsesFound(_logger, responsesList.Count, null);
                
                foreach (var response in responsesList)
                {
                    var chromecast = CreateChromecastReceiver(response);
                    if (chromecast != null)
                    {
                        devices.Add(chromecast);
                        _chromecastDiscovered(_logger, chromecast.Name, chromecast.DeviceUri.ToString(), chromecast.Port, null);
                    }
                }
                
                _mdnsDiscoveryCompleted(_logger, devices.Count, null);
            }
            catch (OperationCanceledException ex)
            {
                _mdnsDiscoveryCancelled(_logger, devices.Count, ex);
            }
            catch (TimeoutException ex)
            {
                _mdnsDiscoveryTimedOut(_logger, devices.Count, ex);
            }
            catch (Exception ex)
            {
                _mdnsDiscoveryError(_logger, devices.Count, ex);
            }
            
            return devices;
        }

        /// <summary>
        /// Start continuous discovery that raises events for found devices
        /// </summary>
        /// <param name="scanInterval">Time between scans (default 5 seconds)</param>
        public void StartContinuousDiscovery(TimeSpan? scanInterval = null)
        {
            StopContinuousDiscovery();
            
            var interval = scanInterval ?? TimeSpan.FromSeconds(5);
            _continuousSearchCts = new CancellationTokenSource();
            
            _continuousDiscoveryStarted(_logger, interval.TotalSeconds, null);
            
            Task.Run(async () =>
            {
                var seenDevices = new HashSet<string>();
                
                while (!_continuousSearchCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        _continuousDiscoveryScanStarted(_logger, null);
                        
                        var responses = await ZeroconfResolver.ResolveAsync(
                            "_googlecast._tcp.local.", 
                            TimeSpan.FromSeconds(5), 
                            cancellationToken: _continuousSearchCts.Token).ConfigureAwait(false);
                        
                        var responsesList = responses.ToList();
                        _continuousDiscoveryScanFound(_logger, responsesList.Count, null);
                        
                        foreach (var response in responsesList)
                        {
                            var chromecast = CreateChromecastReceiver(response);
                            if (chromecast != null)
                            {
                                var deviceKey = $"{chromecast.DeviceUri}:{chromecast.Port}";
                                
                                if (seenDevices.Add(deviceKey))
                                {
                                    _chromecastDiscovered(_logger, chromecast.Name, chromecast.DeviceUri.ToString(), chromecast.Port, null);
                                    ChromecastReceiverFound?.Invoke(this, new ChromecastReceiverEventArgs(chromecast));
                                }
                            }
                        }
                        
                        await Task.Delay(interval, _continuousSearchCts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (_continuousSearchCts.Token.IsCancellationRequested)
                    {
                        _continuousDiscoveryCancelled(_logger, null);
                        break;
                    }
                    catch (TimeoutException ex)
                    {
                        _timeoutDuringContinuousDiscoveryScan(_logger, ex);
                    }
                    catch (Exception ex)
                    {
                        _unexpectedErrorDuringContinuousDiscoveryScan(_logger, ex);
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), _continuousSearchCts.Token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            _continuousDiscoveryCancelledDuringErrorRecovery(_logger, null);
                            break;
                        }
                    }
                }
                
                _continuousDiscoveryStopped(_logger, null);
            }, _continuousSearchCts.Token);
        }

        /// <summary>
        /// Stop continuous discovery
        /// </summary>
        public void StopContinuousDiscovery()
        {
            if (_continuousSearchCts != null)
            {
                _continuousDiscoveryStopping(_logger, null);
                _continuousSearchCts.Cancel();
                _continuousSearchCts.Dispose();
                _continuousSearchCts = null;
            }
        }

        /// <summary>
        /// Create ChromecastReceiver from Zeroconf data
        /// </summary>
        private ChromecastReceiver? CreateChromecastReceiver(IZeroconfHost host)
        {
            try
            {
                if (host.Services?.Any() != true)
                {
                    _hostNoServices(_logger, host.IPAddress, null);
                    return null;
                }

                var service = host.Services.First();

                // Parse TXT records from the properties
                var txtRecords = new Dictionary<string, string>();
                if (service.Value.Properties != null)
                {
                    foreach (var props in service.Value.Properties)
                    {
                        foreach (var prop in props)
                        {
                            txtRecords[prop.Key] = prop.Value;
                        }
                    }
                }

                if (!txtRecords.TryGetValue("fn", out var name) || string.IsNullOrEmpty(name))
                {
                    _hostMissingFriendlyName(_logger, host.IPAddress, null);
                    return null;
                }

                var uriBuilder = new UriBuilder("https", host.IPAddress);

                return new ChromecastReceiver
                {
                    DeviceUri = uriBuilder.Uri,
                    Name = name,
                    Model = txtRecords.TryGetValue("md", out var model) ? model : "",
                    Version = txtRecords.TryGetValue("ve", out var version) ? version : "",
                    ExtraInformation = txtRecords,
                    Status = txtRecords.TryGetValue("rs", out var status) ? status : "",
                    Port = service.Value.Port
                };
            }
            catch (ArgumentException ex)
            {
                _invalidArgumentCreatingReceiver(_logger, host.IPAddress, ex);
                return null;
            }
            catch (UriFormatException ex)
            {
                _invalidUriFormatCreatingReceiver(_logger, host.IPAddress, ex);
                return null;
            }
            catch (Exception ex)
            {
                _unexpectedErrorCreatingReceiver(_logger, host.IPAddress, ex);
                return null;
            }
        }

        #region Logging
        private static readonly Action<ILogger, string, string, int, Exception?> _chromecastDiscovered =
            LoggerMessage.Define<string, string, int>(
            LogLevel.Information,
            new EventId(1001, nameof(_chromecastDiscovered)),
            "New Chromecast device discovered: {DeviceName} at {DeviceUri}:{Port}");

        // Add a new delegate for logging debug messages
        private static readonly Action<ILogger, Exception?> _continuousDiscoveryCancelled =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(1002, nameof(_continuousDiscoveryCancelled)),
                "Continuous discovery cancelled");

        private static readonly Action<ILogger, double, Exception?> _mdnsDiscoveryStarted =
            LoggerMessage.Define<double>(
                LogLevel.Debug,
                new EventId(1003, nameof(_mdnsDiscoveryStarted)),
                "Starting mDNS discovery with timeout {Timeout}ms");

        private static readonly Action<ILogger, int, Exception?> _mdnsDiscoveryResponsesFound =
            LoggerMessage.Define<int>(
                LogLevel.Debug,
                new EventId(1004, nameof(_mdnsDiscoveryResponsesFound)),
                "Found {ResponseCount} mDNS responses");

        private static readonly Action<ILogger, int, Exception?> _mdnsDiscoveryCompleted =
            LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(1005, nameof(_mdnsDiscoveryCompleted)),
                "mDNS discovery completed. Found {DeviceCount} Chromecast devices");

        private static readonly Action<ILogger, int, Exception?> _mdnsDiscoveryCancelled =
            LoggerMessage.Define<int>(
                LogLevel.Warning,
                new EventId(1006, nameof(_mdnsDiscoveryCancelled)),
                "mDNS discovery was cancelled. Returning {DeviceCount} devices found so far");

        private static readonly Action<ILogger, int, Exception?> _mdnsDiscoveryTimedOut =
            LoggerMessage.Define<int>(
                LogLevel.Warning,
                new EventId(1007, nameof(_mdnsDiscoveryTimedOut)),
                "mDNS discovery timed out. Returning {DeviceCount} devices found so far");

        private static readonly Action<ILogger, int, Exception?> _mdnsDiscoveryError =
            LoggerMessage.Define<int>(
                LogLevel.Warning,
                new EventId(1008, nameof(_mdnsDiscoveryError)),
                "Unexpected error during mDNS discovery. Returning {DeviceCount} devices found so far");

        private static readonly Action<ILogger, double, Exception?> _continuousDiscoveryStarted =
            LoggerMessage.Define<double>(
                LogLevel.Information,
                new EventId(1009, nameof(_continuousDiscoveryStarted)),
                "Starting continuous mDNS discovery with interval {Interval}s");

        private static readonly Action<ILogger, Exception?> _continuousDiscoveryScanStarted =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(1010, nameof(_continuousDiscoveryScanStarted)),
                "Starting continuous discovery scan");

        private static readonly Action<ILogger, int, Exception?> _continuousDiscoveryScanFound =
            LoggerMessage.Define<int>(
                LogLevel.Debug,
                new EventId(1011, nameof(_continuousDiscoveryScanFound)),
                "Continuous discovery scan found {ResponseCount} responses");

        private static readonly Action<ILogger, Exception?> _continuousDiscoveryCancelledDuringErrorRecovery =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(1013, nameof(_continuousDiscoveryCancelledDuringErrorRecovery)),
                "Continuous discovery cancelled during error recovery");

        private static readonly Action<ILogger, Exception?> _continuousDiscoveryStopped =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1014, nameof(_continuousDiscoveryStopped)),
                "Continuous mDNS discovery stopped");

        private static readonly Action<ILogger, Exception?> _continuousDiscoveryStopping =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(1015, nameof(_continuousDiscoveryStopping)),
                "Stopping continuous mDNS discovery");

        private static readonly Action<ILogger, string, Exception?> _hostNoServices =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1016, nameof(_hostNoServices)),
                "Host {HostIP} has no services, skipping");

        private static readonly Action<ILogger, string, Exception?> _hostMissingFriendlyName =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1017, nameof(_hostMissingFriendlyName)),
                "Host {HostIP} missing friendly name (fn), skipping");

        private static readonly Action<ILogger, string, Exception?> _invalidArgumentCreatingReceiver =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1018, nameof(_invalidArgumentCreatingReceiver)),
                "Invalid argument when creating ChromecastReceiver from host {HostIP}");

        private static readonly Action<ILogger, string, Exception?> _invalidUriFormatCreatingReceiver =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1019, nameof(_invalidUriFormatCreatingReceiver)),
                "Invalid URI format when creating ChromecastReceiver from host {HostIP}");

        private static readonly Action<ILogger, string, Exception?> _unexpectedErrorCreatingReceiver =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1020, nameof(_unexpectedErrorCreatingReceiver)),
                "Unexpected error creating ChromecastReceiver from host {HostIP}");

        private static readonly Action<ILogger, Exception?> _timeoutDuringContinuousDiscoveryScan =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(1021, nameof(_timeoutDuringContinuousDiscoveryScan)),
                "Timeout during continuous discovery scan, retrying in 1 second");

        private static readonly Action<ILogger, Exception?> _unexpectedErrorDuringContinuousDiscoveryScan =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(1022, nameof(_unexpectedErrorDuringContinuousDiscoveryScan)),
                "Unexpected error during continuous discovery scan, retrying in 1 second");
        #endregion

        #region IDisposable Implementation

        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopContinuousDiscovery();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
