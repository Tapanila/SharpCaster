using System.Text;
using SharpCaster.Models;
using Xunit;

namespace SharpCaster.Test
{
    public class ProtoBufTester
    {
        [Fact]
        public void Connect_Should_Be_Serialized_Well()
        {
            var connectMessage = MessageFactory.Connect();
            connectMessage.Namespace = "urn:x-cast:com.google.cast.tp.connection";
            var bytes = connectMessage.ToProto();
            var expectedBytes = new byte[]
            {
                0, 0, 0, 88, 8, 0, 18, 8, 115, 101, 110, 100, 101, 114, 45, 48, 26, 10, 114, 101, 99, 101, 105, 118,
                101, 114, 45, 48, 34, 40, 117, 114, 110, 58, 120, 45, 99, 97, 115, 116, 58, 99, 111, 109, 46, 103, 111,
                111, 103, 108, 101, 46, 99, 97, 115, 116, 46, 116, 112, 46, 99, 111, 110, 110, 101, 99, 116, 105, 111,
                110, 40, 0, 50, 18, 123, 34, 116, 121, 112, 101, 34, 58, 34, 67, 79, 78, 78, 69, 67, 84, 34, 125
            };
            Assert.Equal(bytes, expectedBytes);
        }

        [Fact]
        public void Ping_Should_Be_Serialized_Well()
        {
            var pingMessage = MessageFactory.Ping;
            var bytes = pingMessage.ToProto();
            var expectedBytes = new byte[]
            {
                0, 0, 0, 43, 8, 0, 18, 8, 115, 101, 110, 100, 101, 114, 45, 48, 26, 10, 114, 101, 99, 101, 105, 118, 101,
                114, 45, 48, 40, 0, 50, 15, 123, 34, 116, 121, 112, 101, 34, 58, 34, 80, 73, 78, 71, 34, 125
            };
            Assert.Equal(bytes, expectedBytes);
        }

        [Fact]
        public void Close_Should_Be_Serialized_Well()
        {
            var closeMessage = MessageFactory.Close;
            var bytes = closeMessage.ToProto();
            var expectedBytes = new byte[]
            {
                0, 0, 0, 44, 8, 0, 18, 8, 115, 101, 110, 100, 101, 114, 45, 48, 26, 10, 114, 101, 99, 101, 105, 118, 101,
                114, 45, 48, 40, 0, 50, 16, 123, 34, 116, 121, 112, 101, 34, 58, 34, 67, 76, 79, 83, 69, 34, 125
            };
            Assert.Equal(bytes, expectedBytes);
        }

        [Fact]
        public void Volume_Level_Should_Be_Serialized_Well()
        {
            var volumeLevel = MessageFactory.Volume(0.7, 100);
            var bytes = volumeLevel.ToProto();
            var expectedBytes = new byte[]
            {
                0, 0, 0, 88, 8, 0, 18, 8, 115, 101, 110, 100, 101, 114, 45, 48, 26, 10, 114, 101, 99, 101, 105, 118, 101,
                114, 45, 48, 40, 0, 50, 60, 123, 34, 118, 111, 108, 117, 109, 101, 34, 58, 123, 34, 108, 101, 118, 101,
                108, 34, 58, 48, 46, 55, 125, 44, 34, 114, 101, 113, 117, 101, 115, 116, 73, 100, 34, 58, 49, 48, 48, 44,
                34, 116, 121, 112, 101, 34, 58, 34, 83, 69, 84, 95, 86, 79, 76, 85, 77, 69, 34, 125
            };
            Assert.Equal(bytes, expectedBytes);
        }
        [Fact]
        public void Volume_Mute_Should_Be_Serialized_Well()
        {
            var volumeMuted = MessageFactory.Volume(true, 100);
            var bytes = volumeMuted.ToProto();
            var expectedBytes = new byte[]
            {
                0, 0, 0, 89, 8, 0, 18, 8, 115, 101, 110, 100, 101, 114, 45, 48, 26, 10, 114, 101, 99, 101, 105, 118, 101,
                114, 45, 48, 40, 0, 50, 61, 123, 34, 118, 111, 108, 117, 109, 101, 34, 58, 123, 34, 109, 117, 116, 101,
                100, 34, 58, 116, 114, 117, 101, 125, 44, 34, 114, 101, 113, 117, 101, 115, 116, 73, 100, 34, 58, 49, 48,
                48, 44, 34, 116, 121, 112, 101, 34, 58, 34, 83, 69, 84, 95, 86, 79, 76, 85, 77, 69, 34, 125
            };
            Assert.Equal(bytes, expectedBytes);
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
