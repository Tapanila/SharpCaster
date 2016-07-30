using SharpCaster.Models;
using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface IChromecastSocketService
    {
        Task Initialize(string host, string port, ChromecastChannel connectionChannel);
        Task Write(byte[] bytes);
    }
}
