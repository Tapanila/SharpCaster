using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Spotify
{
    public class SpotifyStatus
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        [JsonPropertyName("remoteName")]
        public string RemoteName { get; set; }

        [JsonPropertyName("deviceType")]
        public string DeviceType { get; set; }

        [JsonPropertyName("brandDisplayName")]
        public string BrandDisplayName { get; set; }

        [JsonPropertyName("modelDisplayName")]
        public string ModelDisplayName { get; set; }

        [JsonPropertyName("libraryVersion")]
        public string LibraryVersion { get; set; }

        [JsonPropertyName("resolverVersion")]
        public string ResolverVersion { get; set; }

        [JsonPropertyName("groupStatus")]
        public string GroupStatus { get; set; }

        [JsonPropertyName("deviceAPI_isGroup")]
        public bool DeviceAPI_isGroup { get; set; }

        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; }

        [JsonPropertyName("clientID")]
        public string ClientID { get; set; }

        [JsonPropertyName("productID")]
        public int ProductID { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("availability")]
        public string Availability { get; set; }

        [JsonPropertyName("spotifyError")]
        public int SpotifyError { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("statusString")]
        public string StatusString { get; set; }
    }
}
