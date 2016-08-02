using System;
using System.Threading.Tasks;
using SharpCaster.Models;

namespace SharpCaster.Services
{
    public class ChromecastService : IChromecastService
    {
        public static ChromecastService Current
        {
            get
            {
                throw new NotImplementedException();
            }
           
    }

        public DeviceLocator DeviceLocator { get; }
        public ChromeCastClient ChromeCastClient { get; }
        public Chromecast ConnectedChromecast { get; set; }
        

        public void ConnectToChromecast(Chromecast chromecast)
        {
            throw new NotImplementedException();
        }


        public void StopLocatingDevices()
        {
            throw new NotImplementedException();
        }

        public Task StartLocatingDevices()
        {
            throw new NotImplementedException();
        }

        ChromecastService IChromecastService.Current => Current;
    }
}
