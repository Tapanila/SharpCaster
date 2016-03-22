using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using SharpCaster.Models;

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
                Stream bufStream = new MemoryStream();
                bufStream.Write(array, 0, array.Length);
                bufStream.Position = 0;
                var msg = Serializer.Deserialize<CastMessage>(bufStream);
                return msg;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
