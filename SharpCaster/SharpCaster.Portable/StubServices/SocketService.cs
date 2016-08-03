using SharpCaster.Interfaces;
using System;
using System.Threading.Tasks;

namespace SharpCaster.Services
{
    public class SocketService : ISocketService
    {
        public event EventHandler<string> MessageReceived;

        public Task BindEndpointAsync(string localHostName, string localServiceName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        
        public Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public ISocketService Initialize()
        {
            throw new NotImplementedException();
        }

        public void JoinMulticastGroup(string multicastIP)
        {
            throw new NotImplementedException();
        }

        public Task Write(string request, string multicastPort, string multicastIP)
        {
            throw new NotImplementedException();
        }
    }
}
