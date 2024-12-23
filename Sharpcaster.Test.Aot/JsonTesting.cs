using Sharpcaster.Extensions;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Messages.Heartbeat;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sharpcaster.Test.Aot
{
    [TestClass]
    public sealed class JsonTesting
    {
        [TestMethod]
        public void TestConnectMessageSerialization()
        {
            IMessage connectMessage = new ConnectMessage();
            var requestId = ((IMessageWithId)connectMessage).RequestId;

            var output = JsonSerializer.Serialize(connectMessage, SharpcasteSerializationContext.Default.ConnectMessage);
            Assert.AreEqual("{\"requestId\":" + requestId + ",\"type\":\"CONNECT\"}", output);
        }

        [TestMethod]
        public void TestPingMessageSerialization()
        {
            IMessage pingMessage = new PingMessage();

            var output = JsonSerializer.Serialize(pingMessage, SharpcasteSerializationContext.Default.PingMessage);
            Assert.AreEqual("{\"type\":\"PING\"}", output);
        }

        [TestMethod]
        public void TestPingMessageDeserialization()
        {
            const string input = "{\"type\":\"PING\"}";
            var message = JsonSerializer.Deserialize(input, SharpcasteSerializationContext.Default.PingMessage);

            Assert.AreEqual(message?.Type, "PING");
        }
    }
}
