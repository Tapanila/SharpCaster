using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Controllers
{
    public interface  IController
    {
        string ApplicationId { get; set; }
        Task LaunchApplication();
    }
}
