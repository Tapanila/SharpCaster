﻿using Sharpcaster.Test.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using Sharpcaster.Models.Media;
using Sharpcaster.Models;
using System.Threading;
using Sharpcaster.Channels;

namespace Sharpcaster.Test
{
    public class SpotifyTester : IClassFixture<ChromecastDevicesFixture>
    {
        private ITestOutputHelper output;

        public SpotifyTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        //[MemberData(nameof(ChromecastReceiversFilter.GetGoogleCastGroup), MemberType = typeof(ChromecastReceiversFilter))]
        //[MemberData(nameof(CCDevices.GetJblSpeaker), MemberType = typeof(CCDevices))]
        [MemberData(nameof(ChromecastReceiversFilter.GetChromecastUltra), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestChromecastGetInfo(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var SpotifyStatusUpdated = false;
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output, receiver, "CC32E753");
            client.GetChannel<SpotifyChannel>().SpotifyStatusUpdated += (sender, status) =>
            {
                output.WriteLine("ClientId: " + status.ClientID);
                SpotifyStatusUpdated = true;
            };

            await client.GetChannel<SpotifyChannel>().GetSpotifyInfo();
            await Task.Delay(300);
            await client.ReceiverChannel.StopApplication();
            Assert.True(SpotifyStatusUpdated);
        }
    }
}
