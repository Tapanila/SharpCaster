using System;
using System.Threading.Tasks;

namespace Sharpcaster.Models
{
    public class SharpCasterTaskCompletionSource
    {
        private readonly TaskCompletionSource<string> _tcs;

        public Task<string> Task { get => _tcs.Task; }

        public SharpCasterTaskCompletionSource()
        {
            _tcs = new TaskCompletionSource<string>();
        }

        public void SetResult(string parameter)
        {
            _tcs.SetResult(parameter);
        }

        public void SetException(Exception exception)
        {
            _tcs.SetException(exception);
        }
    }

    public enum TaskCompletionMethod
    {
        SetResult,
        SetException
    }
}
