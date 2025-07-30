using System;
using System.IO;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Sharpcaster.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadAsync(this SslStream stream, int bufferLength, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[bufferLength];

            #if NETSTANDARD2_0
            int bytesRead, totalBytesRead = 0;
            while (totalBytesRead < bufferLength)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException(nameof(stream));
                }
                bytesRead = await stream.ReadAsync(buffer, totalBytesRead, bufferLength - totalBytesRead, cancellationToken);
                if (bytesRead == 0)
                {
                    throw new EndOfStreamException();
                }
                totalBytesRead += bytesRead;
            }
            #else
            ArgumentNullException.ThrowIfNull(stream);
            await stream.ReadExactlyAsync(buffer.AsMemory(0, bufferLength), cancellationToken).ConfigureAwait(false);
            #endif
            return buffer;
        }
    }
}