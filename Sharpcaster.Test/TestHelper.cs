using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    public static class TestHelper
    {
        public static List<string> LogContent = new List<string>();
        private static ITestOutputHelper TestOutput = null;


        public async static Task<ChromecastReceiver> FindChromecast()
        {
            IChromecastLocator locator = new MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            return chromecasts.Where(cc=>cc.Name.StartsWith("B")).First();
        }

        public async static Task<ChromecastReceiver> FindChromecast(string name, double timeoutSeconds)
        {
            IChromecastLocator locator = new MdnsChromecastLocator();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
            var chromecasts = await locator.FindReceiversAsync(cts.Token);
            return chromecasts.First(x => x.Name == name);
        }

        public static ChromecastClient GetClientWithConsoleLogging(ITestOutputHelper output, bool makeLogAssertable = false) {

            TestOutput = output;
            var logger = new Mock<ILogger<ChromecastClient>>();

            if (makeLogAssertable) {
                // TODO: we need an instabnce per test here, when ever going back to parallel tests?  (-> test server iso real device )....
                LogContent = new List<string>();
            }

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation => {
                    var logLevel = (LogLevel)invocation.Arguments[0]; // The first two will always be whatever is specified in the setup above
                    var eventId = (EventId)invocation.Arguments[1];  // so I'm not sure you would ever want to actually use them
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });

                    if (makeLogAssertable) {
                        LogContent.Add(logMessage);
                    }
                    TestOutput.WriteLine(logMessage);
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
                                  select t)) {
                serviceCollection.AddTransient(messageInterfaceType, type);
            }

            return new ChromecastClient(serviceCollection);

        }

    }
}
