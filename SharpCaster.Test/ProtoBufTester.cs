using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCaster.Models;
using Xunit;

namespace SharpCaster.Test
{
    public class ProtoBufTester
    {
        [Fact]
        public void TestProtoBuf()
        {
            var connectMessage = MessageFactory.Connect();
            connectMessage.Namespace = "urn:x-cast:com.google.cast.tp.connection";
            var bytes = connectMessage.ToProto();
            Assert.Equal(bytes, new byte[] { 0, 0, 0, 88, 8, 0, 18, 8, 115, 101, 110, 100, 101, 114, 45, 48, 26, 10, 114, 101, 99, 101, 105, 118, 101, 114, 45, 48, 34, 40, 117, 114, 110, 58, 120, 45, 99, 97, 115, 116, 58, 99, 111, 109, 46, 103, 111, 111, 103, 108, 101, 46, 99, 97, 115, 116, 46, 116, 112, 46, 99, 111, 110, 110, 101, 99, 116, 105, 111, 110, 40, 0, 50, 18, 123, 34, 116, 121, 112, 101, 34, 58, 34, 67, 79, 78, 78, 69, 67, 84, 34, 125 });
        }

        private static string PrintBytes(byte[] byteArray)
        {
            var sb = new StringBuilder("new byte[] { ");
            for (var i = 0; i < byteArray.Length; i++)
            {
                var b = byteArray[i];
                sb.Append(b);
                if (i < byteArray.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
