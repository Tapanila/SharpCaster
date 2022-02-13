using Sharpcaster.Models.ChromecastStatus;
using System.Threading.Tasks;

namespace Sharpcaster.Interfaces
{
    public interface IChromecastClient
    {
        //TODO: Write summaries here
        Task SendAsync(string ns, IMessage message, string destinationId);
        Task<ChromecastStatus> LaunchApplicationAsync(string applicationId, bool joinExistingApplicationSession = true);
        Task<TResponse> SendAsync<TResponse>(string ns, IMessageWithId message, string destinationId) where TResponse : IMessageWithId;
        Task DisconnectAsync();
        ChromecastStatus GetChromecastStatus();
    }
}
