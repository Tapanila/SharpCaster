using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;
using SharpCaster.Interfaces;

namespace SharpCaster.Console
{
    class Program
    {
        static readonly ChromecastService _chromecastService = ChromecastService.Current;
        
        static void Main(string[] args)
        {
            
#pragma warning disable 4014
            _chromecastService.StartLocatingDevices();
            System.Console.WriteLine("Started locating chromecasts!");
#pragma warning restore 4014
            _chromecastService.DeviceLocator.DeviceFound += DeviceLocator_DeviceFound;
            _chromecastService.ChromeCastClient.ApplicationStarted += Client_ApplicationStarted;
            _chromecastService.ChromeCastClient.VolumeChanged += _client_VolumeChanged;
            _chromecastService.ChromeCastClient.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
            _chromecastService.ChromeCastClient.Connected += ChromeCastClient_Connected;

            var input = System.Console.ReadLine();
        }

        private static void DeviceLocator_DeviceFound(object sender, Chromecast e)
        {
            if (!e.FriendlyName.Contains("CC"))
            {
                return;
            }
            _chromecastService.StopLocatingDevices();
            System.Console.WriteLine("Device found " + e.FriendlyName);
            _chromecastService.ConnectToChromecast(e);
        }

        private static async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            await _chromecastService.ChromeCastClient.LaunchApplication("B3419EF5");
            System.Console.WriteLine("Connected to chromecast");
        }

        private static void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
        }

        private static void _client_VolumeChanged(object sender, Volume e)
        {
        }

        private static async void Client_ApplicationStarted(object sender, ChromecastApplication e)
        {
            System.Console.WriteLine($"Application {e.DisplayName} has launched");

            var mediaController = _chromecastService.ChromeCastClient.MediaController;

            if (mediaController.SupportsCommand(SupportedCommand.LoadSmoothStreaming))
            {
                await mediaController.LoadSmoothStreaming("http://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/BigBuckBunny.mpd");
            }
        }
    }
}
