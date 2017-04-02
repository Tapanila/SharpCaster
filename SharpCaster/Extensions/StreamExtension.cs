using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpCaster.Extensions
{
    public static class StreamExtension
    {
        public static async Task<IEnumerable<byte>> ParseData(this Stream stream, CancellationToken cancellationToken)
        {
            var sizeBuffer = new byte[4];
            // First message should contain the size of message
            await stream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length, cancellationToken);
            // The message is little-endian (that is, little end first),
            // reverse the byte array.
            Array.Reverse(sizeBuffer);
            //Retrieve the size of message
            var messageSize = BitConverter.ToInt32(sizeBuffer,0);
            var messageBuffer = new byte[messageSize];
            await stream.ReadAsync(messageBuffer, 0, messageBuffer.Length, cancellationToken);
            return messageBuffer;
        }
    }
}
