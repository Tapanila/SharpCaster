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
            _tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public void SetResult(string parameter)
        {
            _tcs.TrySetResult(parameter);
        }

        public void SetException(Exception exception)
        {
            _tcs.TrySetException(exception);
        }
    }
}
