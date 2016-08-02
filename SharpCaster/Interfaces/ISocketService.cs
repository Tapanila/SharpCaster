using System;
using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface ISocketService : IDisposable
    {
        event EventHandler<string> MessageReceived;
        Task BindEndpointAsync(string localHostName, string localServiceName);
        void JoinMulticastGroup(string multicastIP);
        ISocketService Initialize();
        Task<string> GetStringAsync(Uri uri, TimeSpan timeout);
        Task Write(string request, string multicastPort, string multicastIP);
    }
}
