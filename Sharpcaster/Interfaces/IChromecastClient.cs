using Microsoft.Extensions.Logging;
using Sharpcaster.Models.ChromecastStatus;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    public interface IChromecastClient
    {
        Task<ChromecastStatus> LaunchApplicationAsync(string applicationId, bool joinExistingApplicationSession = true);
        Task<string> SendAsync(ILogger logger, string ns, int messageRequestId, string messagePayload, string destinationId);
        Task SendAsync(ILogger logger, string ns, string messagePayload, string destinationId);
        Task<string> WaitResponseAsync(int messageRequestId);
        Task DisconnectAsync();
        ChromecastStatus GetChromecastStatus();
        string FriendlyName { get; set; }
    }
}
