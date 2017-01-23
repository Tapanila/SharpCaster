using System.Threading.Tasks;
using SharpCaster.Interfaces;

namespace SharpCaster.Controllers
{
    public class BaseController : IController
    {
        public string ApplicationId { get; set; }
        public async Task LaunchApplication()
        {
            await Client.ReceiverChannel.LaunchApplication(ApplicationId);
        }

        protected readonly ChromeCastClient Client;

        public BaseController(ChromeCastClient client, string applicationId)
        {
            Client = client;
            ApplicationId = applicationId;
        }

        public async Task StopApplication()
        {
            await Client.ReceiverChannel.StopApplication();
        }

    }
}
