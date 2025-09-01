using Microsoft.Extensions.Logging;
using Moq;
using Sharpcaster.Channels;
using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sharpcaster.Test.helper
{
    public class TestContext
    {
        public List<string> AssertableTestLog = null;
        public ITestOutputHelper TestOutput = null;
    }

    public class TestHelper
    {

        public List<string> AssertableTestLog = null;
        private ITestOutputHelper TestOutput = null;

        public ChromecastReceiver FindChromecast(ChromecastDevicesFixture fixture, string receiverName = null)
        {
            ChromecastReceiver receiver = null;

            var chromecasts = fixture.Receivers;
            if (receiverName == null || receiverName == "*")
            {
                receiver = chromecasts.First();
            }
            else
            {
                receiver = chromecasts.Where(c => c.Name.StartsWith(receiverName)).First();
            }
            try
            {
                TestOutput?.WriteLine("Using Receiver '" + (receiver?.Model ?? "<null>") + "' at " + receiver?.DeviceUri);
            }
            catch
            {
                // If a test does not create a new ITestOutputHelper the old one gets used here and throws 
                // "InvalidOperationException : There is no currently active test."
            }
            return receiver;
        }

        public async Task<ChromecastClient> CreateAndConnectClient(ITestOutputHelper output, ChromecastDevicesFixture fixture, string receiverName = null)
        {
            TestOutput = output;
            var chromecast = FindChromecast(fixture, receiverName);
            ChromecastClient cc = GetClientWithTestOutput(output);
            await cc.ConnectChromecast(chromecast);
            return cc;
        }

        public async Task<ChromecastClient> CreateAndConnectClient(ITestOutputHelper output, ChromecastReceiver receiver)
        {
            TestOutput = output;
            TestOutput?.WriteLine("Using Receiver '" + receiver.Model + "' at " + receiver.DeviceUri + " Name: '" + receiver.Name + "' Version: " + receiver.Version);
            ChromecastClient cc = GetClientWithTestOutput(output);
            await cc.ConnectChromecast(receiver);
            return cc;
        }

        public async Task<ChromecastClient> CreateConnectAndLoadAppClient(ITestOutputHelper output, ChromecastDevicesFixture fixture, string appId = "B3419EF5")
        {
            TestOutput = output;
            ChromecastClient cc = await CreateAndConnectClient(output, fixture);
            await cc.LaunchApplicationAsync(appId, false);
            return cc;
        }

        public async Task<ChromecastClient> CreateConnectAndLoadAppClient(ITestOutputHelper output, ChromecastReceiver receiver, string appId = "B3419EF5")
        {
            TestOutput = output;
            ChromecastClient cc = await CreateAndConnectClient(output, receiver);
            await cc.LaunchApplicationAsync(appId, false);
            return cc;
        }

        public async Task<ChromecastClient> CreateConnectAndLoadAppClient(ChromecastDevicesFixture fixture, string appId = "B3419EF5")
        {
            TestOutput = null;
            var chromecast = FindChromecast(fixture);
            ChromecastClient cc = new();
            await cc.ConnectChromecast(chromecast);
            await cc.LaunchApplicationAsync(appId, false);
            return cc;
        }

        public ChromecastClient GetClientWithTestOutput(ITestOutputHelper output, List<string> assertableLog = null)
        {
            TestOutput = output;
            ILoggerFactory lFactory = CreateMockedLoggerFactory(assertableLog);

            return new ChromecastClient(logger: lFactory.CreateLogger<ChromecastClient>());
        }

        private Mock<ILogger<T>> CreateILoggerMock<T>()
        {
            Mock<ILogger<T>> retVal = new();
            retVal.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            retVal.Setup(x => x.Log(
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
                    var logMessage = (string)invokeMethod?.Invoke(formatter, [state, exception]);

                    var testingName = typeof(T).GetGenericArguments().FirstOrDefault()?.Name;

                    try
                    {
                        TestOutput?.WriteLine(DateTime.Now.ToLongTimeString() + " " + typeof(T).GetGenericArguments().FirstOrDefault()?.Name + " " + logLevel + " " + logMessage);
                    }
                    catch { }
                    AssertableTestLog?.Add(logMessage);
                }));

            return retVal;
        }

        public ILoggerFactory CreateMockedLoggerFactory(List<string> assertableLog = null)
        {
            AssertableTestLog = assertableLog;

            var loggerGeneric = new Mock<ILogger>();
            loggerGeneric.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            loggerGeneric.Setup(x => x.Log(
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
                    var logMessage = (string)invokeMethod?.Invoke(formatter, [state, exception]);

                    try
                    {
                        TestOutput?.WriteLine(DateTime.Now.ToLongTimeString() + " GenericMocked " + logLevel + " " + logMessage);
                    }
                    catch { }
                    AssertableTestLog?.Add(logMessage);
                }));

            var loggerFactory = new Mock<ILoggerFactory>();

            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns
                (new InvocationFunc(new Func<IInvocation, ILogger>((inv) =>
                {
                    var name = (string)inv.Arguments[0];
                    if (name == "Sharpcaster.ChromecastClient")
                    {
                        return CreateILoggerMock<ILogger<ChromecastClient>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.HeartbeatChannel")
                    {
                        return CreateILoggerMock<ILogger<HeartbeatChannel>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.ChromecastChannel")
                    {
                        return CreateILoggerMock<ILogger<ChromecastChannel>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.ConnectionChannel")
                    {
                        return CreateILoggerMock<ILogger<ConnectionChannel>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.MediaChannel")
                    {
                        return CreateILoggerMock<ILogger<MediaChannel>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.ReceiverChannel")
                    {
                        return CreateILoggerMock<ILogger<ReceiverChannel>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.MultiZoneChannel")
                    {
                        return CreateILoggerMock<ILogger<MultiZoneChannel>>().Object;
                    }
                    else if (name == "Sharpcaster.Channels.SpotifyChannel")
                    {
                        return CreateILoggerMock<ILogger<SpotifyChannel>>().Object;
                    }
                    else
                    {
                        return loggerGeneric.Object;
                    }
                }
            )));

            return loggerFactory.Object;
        }

        public static QueueItem[] CreateTestCd
        {
            get
            {
                QueueItem[] MyCd =
                [

                new QueueItem()
                {
                    Media = new Media
                    {
                        ContentId = "Aquarium",
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Aquarium.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
                new QueueItem()
                {
                    Media = new Media
                    {
                        ContentId = "Arcane",
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Arcane.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
                    new QueueItem()
                {
                    Media = new Media
                    {
                        ContentId = "A Mission",
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/A%20Mission.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
                new QueueItem()
                {
                    Media = new Media
                    {
                        ContentId = "All This",
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/All%20This.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
            ];
                return MyCd;
            }
        }

        public static QueueItem[] CreateTestCdWithItemIds
        {
            get
            {
                QueueItem[] MyCd =
                [
                    new QueueItem()
                {
                    ItemId = 0,
                    Media = new Media
                    {
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/A%20Mission.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
                new QueueItem()
                {
                    ItemId = 1,
                    Media = new Media
                    {
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/All%20This.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
                new QueueItem()
                {
                    ItemId = 2,
                    Media = new Media
                    {
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Aquarium.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
                new QueueItem()
                {
                    ItemId = 3,
                    Media = new Media
                    {
                        ContentUrl = "https://incompetech.com/music/royalty-free/mp3-royaltyfree/Arcane.mp3",
                        ContentType = "audio/mpeg"
                    }
                },
            ];
                return MyCd;
            }
        }

        public static QueueItem[] CreateFailingQueue
        {
            get
            {
                QueueItem[] queueItems =
                [

                    new QueueItem()
                {
                    Media = new Media
                    {
                        ContentUrl = "https://audionautix.com/Music/AwayInAManger.mp33",
                        Metadata = new MediaMetadata
                        {
                            Title = "Away In A Manger",
                            SubTitle = "Christmas Songs"
                        }
                    }
                },
                new QueueItem()
                {
                    Media = new Media
                    {
                        ContentUrl = "https://audionautix.com/Music/CarolOfTheBells.mp33",
                        Metadata = new MediaMetadata
                        {
                            Title = "Carol Of The Bells",
                            SubTitle = "Christmas Songs"
                        }
                    }
                },
                new QueueItem()
                {
                    Media = new Media
                    {
                        ContentUrl = "https://audionautix.com/Music/JoyToTheWorld.mp33",
                        Metadata = new MediaMetadata
                        {
                            Title = "Joy To The World",
                            SubTitle = "Christmas Songs"
                        }
                    }
                }

                ];
                return queueItems;
            }
        }
    }
}
