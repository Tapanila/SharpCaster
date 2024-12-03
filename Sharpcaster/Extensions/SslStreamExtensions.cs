using System;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;

namespace Sharpcaster.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadAsync(this SslStream stream, int bufferLength)
        {
            var buffer = new byte[bufferLength];
            int nb, length = 0;
            while (length < bufferLength)
            {
                if (stream == null)
                {
                    throw new InvalidOperationException();
                }
                nb = await stream.ReadAsync(buffer, length, bufferLength - length);
                if (nb == 0)
                {
                    throw new InvalidOperationException();
                }
                length += nb;
            }
            return buffer;
        }
    }
}
