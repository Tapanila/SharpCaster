using System;
using System.Threading;

namespace SharpCaster.Models.ChromecastRequests
{
    public static class RequestIdProvider
    {
        public static int GetNext()
        {
            return Interlocked.Add(ref currentId, 1);
        }

        private static int currentId = new Random((int)DateTime.Now.Ticks).Next();
    }
}


