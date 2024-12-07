using Extensions.Api.CastChannel;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Models;
using Sharpcaster.Test.customChannel;
using Sharpcaster.Test.helper;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
                Converters = {
                    new IMessageJsonConverter()
                }
            };


            var output = JsonSerializer.Serialize(connectMessage, options);
            Assert.Equal("{\"requestId\":" + requestId +  ",\"type\":\"CONNECT\"}", output);
        }


    }
}
