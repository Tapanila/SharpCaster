using System;
using System.IO;
using System.Linq;
using Extensions.Api.CastChannel;

namespace SharpCaster.Extensions
{
    public static class ByteArrayExtension
    {
        public static byte[] AddHeader(this byte[] array)
        {
            var header = BitConverter.GetBytes((uint)array.Length);
            var dataToSend = header.Reverse().ToList();
            dataToSend.AddRange(array.ToList());
            return dataToSend.ToArray();
        }

        public static CastMessage ToCastMessage(this byte[] array)
        {
            try
            {
                var msg = CastMessage.Parser.ParseFrom(array);
                return msg;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}


