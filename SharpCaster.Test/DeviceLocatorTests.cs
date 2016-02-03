using System;
using Xunit;
using System.Linq;
using SharpCaster.Test.DummyServices;

namespace SharpCaster.Test
{
    public class DeviceLocatorTests
    {
        private DeviceLocator _deviceLocator;
        private DummySocketService _dummySocketService;
        public DeviceLocatorTests()
        {
            _dummySocketService = new DummySocketService();
            _deviceLocator = new DeviceLocator { _socketService = _dummySocketService };
        }

        [Fact]
        public async void NoDevicesFound()
        {
            var devices = await _deviceLocator.LocateDevicesAsync(TimeSpan.FromMilliseconds(200));
            Assert.Equal(devices.Count(), 0);
        }

        [Fact]
        public async void OneChromecastFound()
        {
            _dummySocketService.AddResponse("HTTP / 1.1 200 OK\r\nCACHE - CONTROL: max - age = 1800\r\nDATE: Wed, 03 Feb 2016 18:58:47 GMT\r\nEXT:\r\nLOCATION: http://192.168.5.1:8008/ssdp/device-desc.xml\r\nOPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n01-NLS: cb602198-1cc1-11c2-3124-a0ff531c73fa\r\nSERVER: Linux/3.8.13, UPnP/1.0, Portable SDK for UPnP devices/1.6.18\r\nX-User-Agent: redsonic\r\nST: upnp:rootdevice\r\nUSN: uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa::upnp:rootdevice\r\nBOOTID.UPNP.ORG: 34\r\nCONFIGID.UPNP.ORG: 2\r\n\r\n");
            _dummySocketService.AddDeviceInformationResponse("<?xml version=\"1.0\"?>\r\n<root xmlns=\"urn:schemas-upnp-org:device-1-0\">\r\n  <specVersion>\r\n    <major>1</major>\r\n    <minor>0</minor>\r\n  </specVersion>\r\n  <URLBase>http://192.168.5.1:8008</URLBase>\r\n  <device>\r\n    <deviceType>urn:dial-multiscreen-org:device:dial:1</deviceType>\r\n    <friendlyName>Chromecast</friendlyName>\r\n    <manufacturer>Google Inc.</manufacturer>\r\n    <modelName>Eureka Dongle</modelName>\r\n    <UDN>uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa</UDN>\r\n    <iconList>\r\n      <icon>\r\n        <mimetype>image/png</mimetype>\r\n        <width>98</width>\r\n        <height>55</height>\r\n        <depth>32</depth>\r\n        <url>/setup/icon.png</url>\r\n      </icon>\r\n    </iconList>\r\n    <serviceList>\r\n      <service>\r\n        <serviceType>urn:dial-multiscreen-org:service:dial:1</serviceType>\r\n        <serviceId>urn:dial-multiscreen-org:serviceId:dial</serviceId>\r\n        <controlURL>/ssdp/notfound</controlURL>\r\n        <eventSubURL>/ssdp/notfound</eventSubURL>\r\n        <SCPDURL>http://www.google.com/cast</SCPDURL>\r\n      </service>\r\n    </serviceList>\r\n  </device>\r\n</root>\r\n");
            var devices = await _deviceLocator.LocateDevicesAsync(TimeSpan.FromMilliseconds(200));
            Assert.Equal(devices.Count(), 1);
        }

        [Fact]
        public async void TwoChromecastFound()
        {
            _dummySocketService.AddResponse("HTTP / 1.1 200 OK\r\nCACHE - CONTROL: max - age = 1800\r\nDATE: Wed, 03 Feb 2016 18:58:47 GMT\r\nEXT:\r\nLOCATION: http://192.168.5.1:8008/ssdp/device-desc.xml\r\nOPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n01-NLS: cb602198-1cc1-11c2-3124-a0ff531c73fa\r\nSERVER: Linux/3.8.13, UPnP/1.0, Portable SDK for UPnP devices/1.6.18\r\nX-User-Agent: redsonic\r\nST: upnp:rootdevice\r\nUSN: uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa::upnp:rootdevice\r\nBOOTID.UPNP.ORG: 34\r\nCONFIGID.UPNP.ORG: 2\r\n\r\n");
            _dummySocketService.AddResponse("HTTP / 1.1 200 OK\r\nCACHE - CONTROL: max - age = 1800\r\nDATE: Wed, 03 Feb 2016 18:58:47 GMT\r\nEXT:\r\nLOCATION: http://192.168.5.2:8008/ssdp/device-desc.xml\r\nOPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n01-NLS: cb602198-1cc1-11c2-3124-a0ff531c73fa\r\nSERVER: Linux/3.8.13, UPnP/1.0, Portable SDK for UPnP devices/1.6.18\r\nX-User-Agent: redsonic\r\nST: upnp:rootdevice\r\nUSN: uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa::upnp:rootdevice\r\nBOOTID.UPNP.ORG: 34\r\nCONFIGID.UPNP.ORG: 2\r\n\r\n");
            _dummySocketService.AddDeviceInformationResponse("<?xml version=\"1.0\"?>\r\n<root xmlns=\"urn:schemas-upnp-org:device-1-0\">\r\n  <specVersion>\r\n    <major>1</major>\r\n    <minor>0</minor>\r\n  </specVersion>\r\n  <URLBase>http://192.168.5.1:8008</URLBase>\r\n  <device>\r\n    <deviceType>urn:dial-multiscreen-org:device:dial:1</deviceType>\r\n    <friendlyName>Chromecast</friendlyName>\r\n    <manufacturer>Google Inc.</manufacturer>\r\n    <modelName>Eureka Dongle</modelName>\r\n    <UDN>uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa</UDN>\r\n    <iconList>\r\n      <icon>\r\n        <mimetype>image/png</mimetype>\r\n        <width>98</width>\r\n        <height>55</height>\r\n        <depth>32</depth>\r\n        <url>/setup/icon.png</url>\r\n      </icon>\r\n    </iconList>\r\n    <serviceList>\r\n      <service>\r\n        <serviceType>urn:dial-multiscreen-org:service:dial:1</serviceType>\r\n        <serviceId>urn:dial-multiscreen-org:serviceId:dial</serviceId>\r\n        <controlURL>/ssdp/notfound</controlURL>\r\n        <eventSubURL>/ssdp/notfound</eventSubURL>\r\n        <SCPDURL>http://www.google.com/cast</SCPDURL>\r\n      </service>\r\n    </serviceList>\r\n  </device>\r\n</root>\r\n");
            _dummySocketService.AddDeviceInformationResponse("<?xml version=\"1.0\"?>\r\n<root xmlns=\"urn:schemas-upnp-org:device-1-0\">\r\n  <specVersion>\r\n    <major>1</major>\r\n    <minor>0</minor>\r\n  </specVersion>\r\n  <URLBase>http://192.168.5.2:8008</URLBase>\r\n  <device>\r\n    <deviceType>urn:dial-multiscreen-org:device:dial:1</deviceType>\r\n    <friendlyName>Chromecast</friendlyName>\r\n    <manufacturer>Google Inc.</manufacturer>\r\n    <modelName>Eureka Dongle</modelName>\r\n    <UDN>uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa</UDN>\r\n    <iconList>\r\n      <icon>\r\n        <mimetype>image/png</mimetype>\r\n        <width>98</width>\r\n        <height>55</height>\r\n        <depth>32</depth>\r\n        <url>/setup/icon.png</url>\r\n      </icon>\r\n    </iconList>\r\n    <serviceList>\r\n      <service>\r\n        <serviceType>urn:dial-multiscreen-org:service:dial:1</serviceType>\r\n        <serviceId>urn:dial-multiscreen-org:serviceId:dial</serviceId>\r\n        <controlURL>/ssdp/notfound</controlURL>\r\n        <eventSubURL>/ssdp/notfound</eventSubURL>\r\n        <SCPDURL>http://www.google.com/cast</SCPDURL>\r\n      </service>\r\n    </serviceList>\r\n  </device>\r\n</root>\r\n");
            var devices = await _deviceLocator.LocateDevicesAsync(TimeSpan.FromMilliseconds(200));
            Assert.Equal(devices.Count(), 2);
        }

        [Fact]
        public async void OneChromecastTwice()
        {
            _dummySocketService.AddResponse("HTTP / 1.1 200 OK\r\nCACHE - CONTROL: max - age = 1800\r\nDATE: Wed, 03 Feb 2016 18:58:47 GMT\r\nEXT:\r\nLOCATION: http://192.168.5.1:8008/ssdp/device-desc.xml\r\nOPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n01-NLS: cb602198-1cc1-11c2-3124-a0ff531c73fa\r\nSERVER: Linux/3.8.13, UPnP/1.0, Portable SDK for UPnP devices/1.6.18\r\nX-User-Agent: redsonic\r\nST: upnp:rootdevice\r\nUSN: uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa::upnp:rootdevice\r\nBOOTID.UPNP.ORG: 34\r\nCONFIGID.UPNP.ORG: 2\r\n\r\n");
            _dummySocketService.AddResponse("HTTP / 1.1 200 OK\r\nCACHE - CONTROL: max - age = 1800\r\nDATE: Wed, 03 Feb 2016 18:58:47 GMT\r\nEXT:\r\nLOCATION: http://192.168.5.1:8008/ssdp/device-desc.xml\r\nOPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n01-NLS: cb602198-1cc1-11c2-3124-a0ff531c73fa\r\nSERVER: Linux/3.8.13, UPnP/1.0, Portable SDK for UPnP devices/1.6.18\r\nX-User-Agent: redsonic\r\nST: upnp:rootdevice\r\nUSN: uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa::upnp:rootdevice\r\nBOOTID.UPNP.ORG: 34\r\nCONFIGID.UPNP.ORG: 2\r\n\r\n");
            _dummySocketService.AddDeviceInformationResponse("<?xml version=\"1.0\"?>\r\n<root xmlns=\"urn:schemas-upnp-org:device-1-0\">\r\n  <specVersion>\r\n    <major>1</major>\r\n    <minor>0</minor>\r\n  </specVersion>\r\n  <URLBase>http://192.168.5.1:8008</URLBase>\r\n  <device>\r\n    <deviceType>urn:dial-multiscreen-org:device:dial:1</deviceType>\r\n    <friendlyName>Chromecast</friendlyName>\r\n    <manufacturer>Google Inc.</manufacturer>\r\n    <modelName>Eureka Dongle</modelName>\r\n    <UDN>uuid:cb602198-1cc1-11c2-3124-a0ff531c73fa</UDN>\r\n    <iconList>\r\n      <icon>\r\n        <mimetype>image/png</mimetype>\r\n        <width>98</width>\r\n        <height>55</height>\r\n        <depth>32</depth>\r\n        <url>/setup/icon.png</url>\r\n      </icon>\r\n    </iconList>\r\n    <serviceList>\r\n      <service>\r\n        <serviceType>urn:dial-multiscreen-org:service:dial:1</serviceType>\r\n        <serviceId>urn:dial-multiscreen-org:serviceId:dial</serviceId>\r\n        <controlURL>/ssdp/notfound</controlURL>\r\n        <eventSubURL>/ssdp/notfound</eventSubURL>\r\n        <SCPDURL>http://www.google.com/cast</SCPDURL>\r\n      </service>\r\n    </serviceList>\r\n  </device>\r\n</root>\r\n");
            var devices = await _deviceLocator.LocateDevicesAsync(TimeSpan.FromMilliseconds(200));
            Assert.Equal(devices.Count(), 1);
        }
    }

   
}
