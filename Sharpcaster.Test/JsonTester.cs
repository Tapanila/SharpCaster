using Microsoft.VisualBasic;
using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Connection;
using System.Text.Json;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class JsonTester
    {
        [Fact]
        public void TestConnectMessage()
        {
            IMessage connectMessage = new ConnectMessage();
            var requestId = (connectMessage as IMessageWithId).RequestId;

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = SharpcasteSerializationContext.Default
            };


            var output = JsonSerializer.Serialize(connectMessage, options);
            Assert.Equal("{\"requestId\":" + requestId +  ",\"type\":\"CONNECT\"}", output);
        }


    }
}
