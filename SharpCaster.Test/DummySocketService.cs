using SharpCaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;

namespace SharpCaster.Test.DummyServices
{
    public class DummySocketService : ISocketService
    {
        public event EventHandler<string> MessageReceived;
        private List<string> _responses;
        private List<string> _deviceInformationResponses;

        public Task BindEndpointAsync(HostName localHostName, string localServiceName)
        {
            return Task.Delay(1);
        }

        public void Dispose()
        {

        }

        public Task BindEndpointAsync(string localHostName, string localServiceName)
        {
            return Task.Delay(1);
        }

        public void JoinMulticastGroup(string multicastIP)
        {
        }

        public ISocketService Initialize()
        {
            return this;
        }

        public void JoinMulticastGroup(HostName multicastIP)
        {

        }

        public DummySocketService()
        {
            _responses = new List<string>();
            _deviceInformationResponses = new List<string>();
        }
        
        public void AddResponse(string response)
        {
            _responses.Add(response);
        }

        public void AddDeviceInformationResponse(string response)
        {
            _deviceInformationResponses.Add(response);
        }

        public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            var information = _deviceInformationResponses[0];
            _deviceInformationResponses.RemoveAt(0);
            return information;
        }

        public Task Write(string request, string multicastPort, string multicastIP)
        {
            if (_responses.Count > 0)
            {
                foreach (var response in _responses)
                {
                    MessageReceived.Invoke(this, response);
                }
                _responses = new List<string>();
            }
            return Task.Delay(1);
        }
    }
}
