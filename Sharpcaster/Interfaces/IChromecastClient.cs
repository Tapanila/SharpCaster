using Microsoft.Extensions.Logging;
using Sharpcaster.Models.ChromecastStatus;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    public interface IChromecastClient
    {
        //TODO: Write summaries here
        Task SendAsync(ILogger logger, string ns, IMessage message, string destinationId);
        Task<ChromecastStatus> LaunchApplicationAsync(string applicationId, bool joinExistingApplicationSession = true);
        Task<TResponse> SendAsync<TResponse>(ILogger logger, string ns, IMessageWithId message, string destinationId) where TResponse : IMessageWithId;
        Task DisconnectAsync();
        ChromecastStatus GetChromecastStatus();
    }
}
