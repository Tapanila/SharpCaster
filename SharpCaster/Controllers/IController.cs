using System.Threading.Tasks;

namespace SharpCaster.Controllers
{
    public interface  IController
    {
        string ApplicationId { get; set; }
        Task LaunchApplication();
    }
}
