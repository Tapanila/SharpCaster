using System;
using System.IO;
using System.Threading.Tasks;

namespace Sharpcaster.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadAsync(this Stream stream, int bufferLength)
        {
            var buffer = new byte[bufferLength];
            int nb, length = 0;
            while (length < bufferLength)
            {
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
