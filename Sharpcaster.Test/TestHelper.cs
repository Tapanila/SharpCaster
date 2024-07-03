using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    public static class TestHelper
    {
        private static ITestOutputHelper TestOutput = null;
        public static ChromecastReceiver CurrentReceiver { get; private set; }


        public async static Task<ChromecastReceiver> FindChromecast()
        {
            IChromecastLocator locator = new MdnsChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            CurrentReceiver = chromecasts.First();
            try {
                TestOutput?.WriteLine("Using Receiver '" + (CurrentReceiver?.Model ?? "<null>") + "' at " + CurrentReceiver?.DeviceUri);
            } catch {
                // If a test does not create a new ITestOutputHelper the old one gets used here and throws 
                // "InvalidOperationException : There is no currently active test."
            }
            return CurrentReceiver;
        }

        public async static Task<ChromecastClient> CreateAndConnectClient(ITestOutputHelper output) {
            TestOutput = output;
            var chromecast = await TestHelper.FindChromecast();
            ChromecastClient cc = GetClientWithTestOutput(output);
            await cc.ConnectChromecast(chromecast);
            return cc;
        }

        public async static Task<ChromecastClient> CreateConnectAndLoadAppClient(ITestOutputHelper output) {
            TestOutput = output;
            ChromecastClient cc = await CreateAndConnectClient(output);
            await cc.LaunchApplicationAsync("B3419EF5", false);
            return cc;
        }


        public static ChromecastClient GetClientWithTestOutput(ITestOutputHelper output) {

            TestOutput = output;
            var logger = new Mock<ILogger<ChromecastClient>>();

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
                   
                    TestOutput.WriteLine(DateTime.Now.ToLongTimeString() + " " + logMessage);
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

        public static QueueItem[] CreateTestCd() {
            QueueItem[] MyCd = new QueueItem[4];
            MyCd[0] = new QueueItem() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/Frankie%20by%20Mississippi%20John%20Hurt.mp3"
                }
            };
            MyCd[1] = new QueueItem() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/Mississippi%20Boweavil%20Blues%20by%20The%20Masked%20Marvel.mp3"
                }
            };
            MyCd[2] = new QueueItem() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/The%20Wild%20Wagoner%20by%20Jilson%20Setters.mp3"
                }
            };
            MyCd[3] = new QueueItem() {
                Media = new Media {
                    ContentUrl = "http://www.openmusicarchive.org/audio/Drunkards%20Special%20by%20Coley%20Jones.mp3"
                }
            };
            return MyCd;
        }

    }
}
