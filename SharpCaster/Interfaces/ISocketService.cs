using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage.Streams;

namespace SharpCaster.Interfaces
{
    public interface ISocketService : IDisposable
    {
        event EventHandler<string> MessageReceived;
        Task BindEndpointAsync(HostName localHostName, string localServiceName);
        void JoinMulticastGroup(HostName multicastIP);
        ISocketService Initialize();
        Task<DataWriter> GetOutputWriterAsync(HostName multicastIP, string multicastPort);
        Task<string> GetStringAsync(Uri uri, TimeSpan timeout);
    }
}
