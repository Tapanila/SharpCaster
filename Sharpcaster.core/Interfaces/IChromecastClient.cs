using Sharpcaster.Core.Models.ChromecastStatus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpcaster.Core.Interfaces
{
    public interface IChromecastClient
    {
        //TODO: Write summaries here
        Task SendAsync(string ns, IMessage message, string destinationId);
        Task<ChromecastStatus> LaunchApplicationAsync(string applicationId, bool joinExistingApplicationSession = true);
        Task<TResponse> SendAsync<TResponse>(string ns, IMessageWithId message, string destinationId) where TResponse : IMessageWithId;
        Task DisconnectAsync();
    }
}
