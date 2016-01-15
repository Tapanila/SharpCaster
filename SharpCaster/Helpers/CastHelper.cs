using System;
using System.IO;
using System.Linq;
using ProtoBuf;

namespace SharpCaster.Helpers
{
    public static class CastHelper
    {
        public static byte[] AddHeader(byte[] array)
        {
            var header = BitConverter.GetBytes((uint)array.Length);
            var dataToSend = header.Reverse().ToList();
            dataToSend.AddRange(array.ToList());
            return dataToSend.ToArray();
        }
        public static CastMessage ToCastMessage(byte[] array, bool includeHeader = true)
        {

            try
            {
                //var str = System.Text.Encoding.UTF8.GetString(array,0,array.Length);
                Stream bufStream = new MemoryStream();
                bufStream.Write(array, 0, array.Length);
                bufStream.Position = 0;
                var msg = Serializer.DeserializeWithLengthPrefix<CastMessage>(bufStream, PrefixStyle.Fixed32BigEndian);
                return msg;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static byte[] ToProto(object obj, bool includeHeader = true)
        {

            var bufStream = new MemoryStream();
            Serializer.Serialize(bufStream, obj);

            if (includeHeader)
            {
                var buffer = AddHeader(bufStream.ToArray());
                return buffer;
            }
            else
            {
                return bufStream.ToArray();
            }

        }

    }
}
