using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpCaster.Channels;

namespace SharpCaster.Interfaces
{
    public interface IChromecastSocketService
    {
        Task Initialize(string host, string port, ConnectionChannel connectionChannel, HeartbeatChannel heartbeatChannel, Action<Stream,bool,CancellationToken> packetReader, CancellationToken cancellationToken);
        Task Write(byte[] bytes, CancellationToken cancellationToken);
        Task Disconnect();
    }
}
