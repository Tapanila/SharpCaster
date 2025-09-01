using Sharpcaster.Messages.Receiver;
using Xunit;

namespace Sharpcaster.Test
{
    public class MessageTypeTester()
    {
        [Fact]
        public void TestStopMessageTypeConversion()
        {
            var stopMessage = new StopMessage();
            Assert.Equal("STOP", stopMessage.Type);
        }

        [Fact]
        public void TestLaunchMessageTypeConversion()
        {
            var launchMessage = new LaunchMessage();
            Assert.Equal("LAUNCH", launchMessage.Type);
        }

        [Fact]
        public void TestReceiveStatusMessageTypeConversion()
        {
            var receiveStatus = new ReceiverStatusMessage();
            Assert.Equal("RECEIVER_STATUS", receiveStatus.Type);
        }
    }
}
