using System;
using System.IO;
using System.Threading.Tasks;
using SharpCaster.Channels;
using SharpCaster.Interfaces;

namespace SharpCaster
{
    internal class ChromecastSocketService : IChromecastSocketService
    {
     
        public Task Initialize(string host, string port, ConnectionChannel connectionChannel, HeartbeatChannel heartbeatChannel,
            Action<Stream, bool> packetReader)
        {
            throw new NotImplementedException();
        }

        public Task Write(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}