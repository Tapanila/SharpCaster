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

            var output = JsonSerializer.Serialize(connectMessage, SharpcasteSerializationContext.Default.ConnectMessage);
            Assert.Equal("{\"requestId\":" + requestId +  ",\"type\":\"CONNECT\"}", output);
        }


    }
}
