using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using Sharpcaster.Channels;
using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Models;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;
using Sharpcaster.Test.helper;
using Sharpcaster;
using System.Runtime.CompilerServices;
using Castle.Components.DictionaryAdapter.Xml;






namespace Sharpcaster.Test {

    public class ChromecastReceiversFilter  {

        public static IEnumerable<object[]> GetAll() {
            if (ChromecastDevicesFixture.Receivers.Count > 0) {
                foreach (var cc in ChromecastDevicesFixture.Receivers) {
                    yield return new object[] { cc };
                }
            } else {
                throw new Exception("This Test needs a Chromecast Receiver to be available on local network!");
            }
        }

        public static IEnumerable<object[]> GetAny() {
            var rec = ChromecastDevicesFixture.Receivers.FirstOrDefault();
            if (rec != null) {
                yield return new object[] { rec };
            } else {
                throw new Exception("This Test needs a Chromecast Receiver to be available on local network!");
            }
        }

        public static IEnumerable<object[]> GetJblSpeaker() {
            var rec = ChromecastDevicesFixture.Receivers.Where(r => r.Model.StartsWith("JBL")).FirstOrDefault();
            if (rec != null) {
                yield return new object[] { rec };
            } else {
                yield break;
            }
        }

        public static IEnumerable<object[]> GetDefaultDevice() {
            var rec = ChromecastDevicesFixture.Receivers.Where(r => ! r.Model.StartsWith("JBL")).FirstOrDefault();
            if (rec != null) {
                yield return new object[] { rec };
            } else {
                yield break;
            }
        }

        public static IEnumerable<object[]> GetChromecastUltra()
        {
            var rec = ChromecastDevicesFixture.Receivers.Where(r => r.Model.StartsWith("Chromecast Ultra")).FirstOrDefault();
            if (rec != null) {
                yield return new object[] { rec };
            } else {
                yield break;
            }
        }
        
    }


    public class TestContext {
        public List<string> AssertableTestLog = null;
        public ITestOutputHelper TestOutput = null;
    }


    public class TestHelper
    {

        

        public  List<string> AssertableTestLog = null;
        private ITestOutputHelper TestOutput = null;
        //public static ChromecastReceiver CurrentReceiver { get; private set; }


        public ChromecastReceiver FindChromecast(string receiverName = null)
        {
            //IChromecastLocator locator = new MdnsChromecastLocator();
            ChromecastReceiver receiver = null;

            var chromecasts = ChromecastDevicesFixture.Receivers; //await locator.FindReceiversAsync();
            if ((receiverName == null) || (receiverName == "*")) {
                receiver = chromecasts.First();
            } else {
                receiver = chromecasts.Where(c => c.Name.StartsWith(receiverName)).First();
            }
            try {
                TestOutput?.WriteLine("Using Receiver '" + (receiver?.Model ?? "<null>") + "' at " + receiver?.DeviceUri);
            } catch {
                // If a test does not create a new ITestOutputHelper the old one gets used here and throws 
                // "InvalidOperationException : There is no currently active test."
            }
            return receiver;
        }

        public async Task<ChromecastClient> CreateAndConnectClient(ITestOutputHelper output, string receiverName = null) {
            TestOutput = output;
            var chromecast = FindChromecast(receiverName);
            ChromecastClient cc = GetClientWithTestOutput(output);
            await cc.ConnectChromecast(chromecast);
            return cc;
        }

        public async Task<ChromecastClient> CreateAndConnectClient(ITestOutputHelper output, ChromecastReceiver receiver) {
            TestOutput = output;
            TestOutput?.WriteLine("Using Receiver '" + (receiver.Model) + "' at " + receiver.DeviceUri);
            ChromecastClient cc = GetClientWithTestOutput(output);
            await cc.ConnectChromecast(receiver);
            return cc;
        }


        public async Task<ChromecastClient> CreateConnectAndLoadAppClient(ITestOutputHelper output, string appId = "B3419EF5") {
            TestOutput = output;
            ChromecastClient cc = await CreateAndConnectClient(output);
            await cc.LaunchApplicationAsync(appId, false);
            return cc;
        }

        public async Task<ChromecastClient> CreateConnectAndLoadAppClient(ITestOutputHelper output, ChromecastReceiver receiver, string appId = "B3419EF5") {
            TestOutput = output;
            ChromecastClient cc = await CreateAndConnectClient(output, receiver);
            await cc.LaunchApplicationAsync(appId, false);
            return cc;
        }

        public async Task<ChromecastClient> CreateConnectAndLoadAppClient(string appId = "B3419EF5") {
            TestOutput = null;
            var chromecast = FindChromecast();
            ChromecastClient cc = new ChromecastClient();
            await cc.ConnectChromecast(chromecast);
            await cc.LaunchApplicationAsync(appId, false);
            return cc;
        }


        public ChromecastClient GetClientWithTestOutput(ITestOutputHelper output, List<string> assertableLog = null) {

            TestOutput = output;
            ILoggerFactory lFactory = CreateMockedLoggerFactory(assertableLog);

            return new ChromecastClient(loggerFactory: lFactory);
        }


        private Mock<ILogger<T>> CreateILoggerMock<T>() {
            Mock<ILogger<T>> retVal = new Mock<ILogger<T>>();
            retVal.Setup(x => x.Log(
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

                    var testingName = typeof(T).GetGenericArguments().FirstOrDefault()?.Name;

                    try {
                        TestOutput?.WriteLine(DateTime.Now.ToLongTimeString() + " " + typeof(T).GetGenericArguments().FirstOrDefault()?.Name + " " + logLevel + " " + logMessage);
                    } catch { }
                    AssertableTestLog?.Add(logMessage);
                }));

            return retVal;
        }


        public ILoggerFactory CreateMockedLoggerFactory(List<string> assertableLog = null) {
            AssertableTestLog = assertableLog;

            var loggerGeneric = new Mock<ILogger>();
            loggerGeneric.Setup(x => x.Log(
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

                    try {
                        TestOutput?.WriteLine(DateTime.Now.ToLongTimeString() + " GenericMocked " + logLevel + " " + logMessage);
                    } catch { }
                    AssertableTestLog?.Add(logMessage);
                }));

            //var loggerCC = CreateILoggerMock<ILogger<ChromecastClient>>();
            //var loggerHBC = CreateILoggerMock<ILogger<HeartbeatChannel>>();
            //var loggerCCC = CreateILoggerMock<ILogger<ChromecastChannel>>();


            var loggerFactory = new Mock<ILoggerFactory>();

            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns
                (new InvocationFunc(new Func<IInvocation, ILogger>((inv) => {
                    var name = (string)inv.Arguments[0];
                    if (name == "Sharpcaster.ChromecastClient") {
                        return CreateILoggerMock<ILogger<ChromecastClient>>().Object;
                    } else if (name == "Sharpcaster.Channels.HeartbeatChannel") {
                        return CreateILoggerMock<ILogger<HeartbeatChannel>>().Object;
                    } else if (name == "Sharpcaster.Channels.ChromecastChannel") {
                        return CreateILoggerMock<ILogger<ChromecastChannel>>().Object;
                    } else if (name == "Sharpcaster.Channels.ConnectionChannel") {
                        return CreateILoggerMock<ILogger<ConnectionChannel>>().Object;
                    } else if (name == "Sharpcaster.Channels.MediaChannel") {
                        return CreateILoggerMock<ILogger<MediaChannel>>().Object;
                    } else if (name == "Sharpcaster.Channels.ReceiverChannel") {
                        return CreateILoggerMock<ILogger<ReceiverChannel>>().Object;
                    } else {
                        return loggerGeneric.Object;
                    }
            }
            )));

            return loggerFactory.Object;
        }


        public QueueItem[] CreateTestCd() {
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
