using System;

namespace Sharpcaster.Interfaces
{
    public interface IConsoleWrapper
    {
        void WriteLine(string line);
        void WriteLine(string line, Exception ex, object p);
    }
}
