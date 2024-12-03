﻿using Sharpcaster.Models;
using Sharpcaster.Test.helper;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sharpcaster.Test
{
    [Collection("SingleCollection")]
    public class ReceiverChannelTester : IClassFixture<ChromecastDevicesFixture>
    {
        private ITestOutputHelper output;

        public ReceiverChannelTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestMute(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);

            var status = await client.ReceiverChannel.SetMute(true);

            Assert.True(status.Volume.Muted);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestUnMute(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);

            var status = await client.ReceiverChannel.SetMute(false);
            Assert.False(status.Volume.Muted);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestVolume(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);

            var status = await client.ReceiverChannel.SetVolume(0.1);
            Assert.Equal(0.1, status.Volume.Level.Value, precision: 1);

            await Task.Delay(500);      // My Chromecast Audio device (somtimes) needs this delay here to pass the test, because the first volume requests triggers a lot of responses 
                                        // (some of them on a 'multizone' channel) with eualizer data !? and the 2nd request does not get the new volume but another answer with old 0.1 as volume !!!
                                        // It happens always if this test runs directly after the TestStoppingApplication!?

            status = await client.ReceiverChannel.SetVolume(0.3);
            Assert.Equal(0.3, status.Volume.Level.Value, precision: 1);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAll), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestStoppingApplication(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var client = await TestHelper.CreateAndConnectClient(output, receiver);

            await client.LaunchApplicationAsync("B3419EF5");

            var status = await client.ReceiverChannel.StopApplication();
            Assert.Null(status.Applications);
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestApplicationLaunchStatusMessage(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper(); 
            var client = await TestHelper.CreateAndConnectClient(output, receiver);

            string launchStatus = "";

            client.ReceiverChannel.LaunchStatusChanged += (sender, e) =>
            {
                launchStatus = e.Status;
            };

            await client.LaunchApplicationAsync("B3419EF5");

            var status = await client.ReceiverChannel.StopApplication();
            Assert.Null(status.Applications);
            Assert.Equal("USER_ALLOWED", launchStatus);
        }

        private void ReceiverChannel_LaunchStatusChanged(object sender, Messages.Receiver.LaunchStatusMessage e)
        {
            throw new System.NotImplementedException();
        }
    }
}
