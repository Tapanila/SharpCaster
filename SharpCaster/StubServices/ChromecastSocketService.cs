using System;
using System.IO;
using System.Threading.Tasks;
using SharpCaster.Interfaces;
using SharpCaster.Models;

namespace SharpCaster
{
    public class ChromecastSocketService : IChromecastSocketService
    {
        public Task Initialize(string host, string port, ChromecastChannel connectionChannel, ChromecastChannel heartbeatChannel, Action<Stream,bool> packetReader)
        {
            throw new NotImplementedException();
        }

        public Task Write(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}