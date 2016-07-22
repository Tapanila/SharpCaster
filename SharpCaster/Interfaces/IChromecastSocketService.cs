using SharpCaster.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface IChromecastSocketService
    {
        Task Initialize(string host, string port, ChromecastChannel connectionChannel, ChromecastChannel heartbeatChannel, Action<Stream,bool> packetReader);
        Task Write(byte[] bytes);
    }
}
