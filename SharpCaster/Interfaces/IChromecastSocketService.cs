using SharpCaster.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface IChromecastSocketService
    {
        List<ChromecastChannel> Channels { get; set; }

        Task Initialize(string host, string port);

        Task ReadPackets();

        Task Write(byte[] bytes);
    }
}
