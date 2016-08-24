using System.Threading.Tasks;

namespace SharpCaster.Controllers
{
    public interface  IMediaController : IController
    {
        Task Play();

        Task Pause();

        Task Seek(double seconds);

        Task Stop();

        Task Next();

        Task Previous();

        Task VolumeUp(double amount = 0.05);

        Task VolumeDown(double amount = 0.05);

        Task SetMute(bool muted);

        Task SetVolume(double volume);
    }
}
