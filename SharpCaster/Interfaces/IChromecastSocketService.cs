using System;
using System.IO;
using System.Threading.Tasks;
using SharpCaster.Channels;

namespace SharpCaster.Interfaces
{
    public interface IChromecastSocketService
    {
        Task Initialize(string host, string port, ConnectionChannel connectionChannel, HeartbeatChannel heartbeatChannel, Action<Stream,bool> packetReader);
        Task Write(byte[] bytes);
    }
}
