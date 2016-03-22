using System;
using System.Collections.Generic;
using System.IO;

namespace SharpCaster.Extensions
{
    public static class StreamExtension
    {
        public static IEnumerable<byte> ParseData(this Stream stream)
        {
            var sizeBuffer = new byte[4];
            byte[] messageBuffer = {};
            // First message should contain the size of message
            stream.Read(sizeBuffer, 0, sizeBuffer.Length);
            // The message is little-endian (that is, little end first),
            // reverse the byte array.
            Array.Reverse(sizeBuffer);
            //Retrieve the size of message
            var messageSize = BitConverter.ToInt32(sizeBuffer,0);
            messageBuffer = new byte[messageSize];
            stream.Read(messageBuffer, 0, messageBuffer.Length);
            return messageBuffer;
        }
    }
}
