using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface IReceiverController
    {
        void GetChromecastStatus();
        Task SetVolume(float level);
        Task SetMute(bool muted);
        Task StopApplication();

    }
}
