using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class LoggingTester
    {

        [Fact]
        public void TestLogging() {
            List<string> logLines = new List<string>();
            var loggerFactory = TestHelper.CreateMockedLoggerFactory(logLines);

            var client = new ChromecastClient(loggerFactory: loggerFactory);
            Assert.Equal("[RECEIVER_STATUS,QUEUE_CHANGE,QUEUE_ITEM_IDS,QUEUE_ITEMS,INVALID_REQUEST,LOAD_CANCELLED,LOAD_FAILED,MEDIA_STATUS,PING,CLOSE]", logLines[0]);
        }
    }
}
