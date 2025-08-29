﻿using Sharpcaster.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Sharpcaster.Test.helper;
using System.Linq;

namespace Sharpcaster.Test
{
    public class ChromecastLocatorTester()
    {
        [Fact]
        public async Task SearchChromecasts()
        {
            var locator = new ChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);
        }

        [Fact]
        public async Task SearchChromecastsTrickerEvent()
        {
            int counter = 0;
            var locator = new ChromecastLocator();
            locator.ChromecastReceiverFound += (object sender, ChromecastReceiverEventArgs e) =>
            {
                counter++;
            };
            
            // Start continuous discovery to trigger events
            locator.StartContinuousDiscovery(TimeSpan.FromSeconds(5));
            
            // Wait for events to be fired
            await Task.Delay(TimeSpan.FromMilliseconds(5200), Xunit.TestContext.Current.CancellationToken);
            
            // Stop discovery
            locator.StopContinuousDiscovery();
            
            // Also test async method still works
            var chromecasts = await locator.FindReceiversAsync();
            Assert.NotEmpty(chromecasts);
            Assert.NotEqual(0, counter);
        }

        [Fact]
        public async Task SearchChromecastsWithTooShortTimeout()
        {
            var locator = new ChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync(TimeSpan.FromMicroseconds(1), TimeSpan.FromMicroseconds(1), TimeSpan.FromMicroseconds(1));
            Assert.Empty(chromecasts);
        }

        [Fact]
        public async Task SearchChromecastsWithTimeout()
        {
            var locator = new ChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync(TimeSpan.FromSeconds(5));
            Assert.NotEmpty(chromecasts);
        }

        [Fact]
        public async Task TestNewProgressiveDiscovery()
        {
            var locator = new ChromecastLocator();
            var chromecasts = await locator.FindReceiversAsync();
            Assert.True(chromecasts.Count() >= 3);
        }
    }
}
