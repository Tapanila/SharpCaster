using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpcaster.Core.Interfaces
{
    public interface ILogger
    {
        Task Log(string message);
    }
}
