using Sharpcaster.Test.helper;
using System;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using Sharpcaster.Models;
using Sharpcaster.Channels;
using System.Net.Http;
using System.Net;
using System.Text.Json.Nodes;
using System.Net.Http.Json;

namespace Sharpcaster.Test
{
    public class SpotifyTester : IClassFixture<ChromecastDevicesFixture>
    {
        private readonly ITestOutputHelper output;

        public SpotifyTester(ITestOutputHelper outputHelper, ChromecastDevicesFixture fixture)
        {
            output = outputHelper;
            output.WriteLine("Fixture has found " + ChromecastDevicesFixture.Receivers?.Count + " receivers with " + fixture.GetSearchesCnt() + " searche(s).");
        }

        [Theory]
        [MemberData(nameof(ChromecastReceiversFilter.GetAny), MemberType = typeof(ChromecastReceiversFilter))]
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

        [Theory(Skip = "Requires spotify cookies inserted to code")]
        [MemberData(nameof(ChromecastReceiversFilter.GetChromecastUltra), MemberType = typeof(ChromecastReceiversFilter))]
        public async Task TestChromecastWholeFlow(ChromecastReceiver receiver)
        {
            var TestHelper = new TestHelper();
            var SpotifyStatusUpdated = false;
            var UserAddedToChromecast = false;
            var spotifyClientId = "";
            var spotifyDeviceId = "";

            var spotifyAccessToken = await GetSpotifyAccessToken();
            ChromecastClient client = await TestHelper.CreateConnectAndLoadAppClient(output, receiver, "CC32E753");
            client.GetChannel<SpotifyChannel>().SpotifyStatusUpdated += async (sender, status) =>
            {
                spotifyClientId = status.ClientID;
                spotifyDeviceId = client.GetChannel<SpotifyChannel>().SpotifyDeviceId;
                var spotifyAccessTokenForChromecast = await GetSpotifyAccessTokenForChromecast(spotifyAccessToken, spotifyClientId, spotifyDeviceId);
                await client.GetChannel<SpotifyChannel>().AddUser(spotifyAccessTokenForChromecast);
                SpotifyStatusUpdated = true;
            };

            client.GetChannel<SpotifyChannel>().AddUserResponseReceived += (sender, e) =>
            {
                output.WriteLine("AddUserResponseReceived: " + e.StatusString);
                UserAddedToChromecast = true;
            };

            await client.GetChannel<SpotifyChannel>().GetSpotifyInfo();
            await Task.Delay(1500);
            Assert.True(SpotifyStatusUpdated);
            Assert.True(UserAddedToChromecast);
        }

        public async Task<string> GetSpotifyAccessToken()
        {
            var baseAddress = new Uri("https://open.spotify.com");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                cookieContainer.Add(baseAddress, new Cookie("sp_dc", ""));
                cookieContainer.Add(baseAddress, new Cookie("sp_key", ""));
                client.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");

                var result = await client.GetAsync("/get_access_token?reason=transport&productType=web_player");
                result.EnsureSuccessStatusCode();
                var resultString = await result.Content.ReadAsStringAsync();
                var json = JsonNode.Parse(resultString);
                return json["accessToken"].ToString();
            }
        }

        public async Task<string> GetSpotifyAccessTokenForChromecast(string originalAccessToken, string clientId, string deviceId)
        {
            var baseAddress = new Uri("https://spclient.wg.spotify.com");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add(
                    "authority", "spclient.wg.spotify.com");
                client.DefaultRequestHeaders.Add(
                    "authorization", "Bearer " + originalAccessToken);
                var jsonContent = JsonContent.Create(new
                {
                    clientId,
                    deviceId
                });

                var result = await client.PostAsync("/device-auth/v1/refresh", jsonContent);
                result.EnsureSuccessStatusCode();
                var resultString = await result.Content.ReadAsStringAsync();
                var json = JsonNode.Parse(resultString);
                return json["accessToken"].ToString();
            }
        }
    }
}
