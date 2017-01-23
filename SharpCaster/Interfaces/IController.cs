using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface  IController
    {
        string ApplicationId { get; set; }
        Task LaunchApplication();
    }
}
