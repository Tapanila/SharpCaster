using System;
using System.IO;
using System.Threading.Tasks;
using SharpCaster.Interfaces;
using SharpCaster.Models;

namespace SharpCaster
{
    public class ChromecastSocketService : BaseChromecastSocketService, IChromecastSocketService
    {
        public Task Initialize(string host, string port)
        {
            throw new NotImplementedException();
        }
        
        public Task ReadPackets()
        {
            throw new NotImplementedException();
        }

        public Task Write(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}