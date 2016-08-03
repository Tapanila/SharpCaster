using SharpCaster.Models;
using System.Threading.Tasks;

namespace SharpCaster.Interfaces
{
    public interface IConnectionController
    {
        Task OpenConnection();

        Task<bool> ConnectToApplication(string applicationId);        
    }
}
