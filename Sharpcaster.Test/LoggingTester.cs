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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class LoggingTester
    {
        
        [Fact]
        public void TestLogging()
        {
            var logger = new Mock<ILogger<ChromecastClient>>();

            var logMessageFirst = "";

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0]; // The first two will always be whatever is specified in the setup above
                    var eventId = (EventId)invocation.Arguments[1];  // so I'm not sure you would ever want to actually use them
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });
                    if (logMessageFirst == "")
                    {
                        logMessageFirst = logMessage;
                    }
                }));

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IChromecastChannel, ConnectionChannel>();
            serviceCollection.AddTransient<IChromecastChannel, HeartbeatChannel>();
            serviceCollection.AddTransient<IChromecastChannel, ReceiverChannel>();
            serviceCollection.AddTransient<IChromecastChannel, MediaChannel>();
            serviceCollection.AddSingleton<ILogger>(logger.Object);
            var messageInterfaceType = typeof(IMessage);
            foreach (var type in (from t in typeof(IConnectionChannel).GetTypeInfo().Assembly.GetTypes()
                                  where t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && messageInterfaceType.IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ReceptionMessageAttribute>() != null
                                  select t))
            {
                serviceCollection.AddTransient(messageInterfaceType, type);
            }

            var client = new ChromecastClient(serviceCollection);
            Assert.Equal("[RECEIVER_STATUS,INVALID_REQUEST,LOAD_CANCELLED,LOAD_FAILED,MEDIA_STATUS,QUEUE_CHANGE,QUEUE_ITEM_IDS,QUEUE_ITEMS,PING,CLOSE]",logMessageFirst);
        }
    }
}
