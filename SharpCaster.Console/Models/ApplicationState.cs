using Sharpcaster;
using Sharpcaster.Models;
using Microsoft.Extensions.Logging;

namespace SharpCaster.Console.Models;

public class ApplicationState
{
    public List<ChromecastReceiver> Devices { get; set; } = new();
    public ChromecastReceiver? SelectedDevice { get; set; }
    public ChromecastClient? Client { get; set; }
    public bool IsConnected { get; set; }
    public DateTime LastConnectionCheck { get; set; }
    public ILogger? Logger { get; set; }
    public MdnsChromecastLocator? Locator { get; set; }
    
}