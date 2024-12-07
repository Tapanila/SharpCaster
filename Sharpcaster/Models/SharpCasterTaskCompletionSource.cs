using Sharpcaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpcaster.Models
{
    public class SharpCasterTaskCompletionSource
    {
        private readonly TaskCompletionSource<IMessageWithId> _tcs;

        public Task<IMessageWithId> Task { get => _tcs.Task; }

        public SharpCasterTaskCompletionSource()
        {
            _tcs = new TaskCompletionSource<IMessageWithId>();
        }

        public void SetResult(IMessageWithId parameter)
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
