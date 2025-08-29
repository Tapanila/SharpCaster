using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Spotify
{
    public class SpotifyStatus
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; } = string.Empty;

        [JsonPropertyName("remoteName")]
        public string RemoteName { get; set; } = string.Empty;

        [JsonPropertyName("deviceType")]
        public string DeviceType { get; set; } = string.Empty;

        [JsonPropertyName("brandDisplayName")]
        public string BrandDisplayName { get; set; } = string.Empty;

        [JsonPropertyName("modelDisplayName")]
        public string ModelDisplayName { get; set; } = string.Empty;

        [JsonPropertyName("libraryVersion")]
        public string LibraryVersion { get; set; } = string.Empty;

        [JsonPropertyName("resolverVersion")]
        public string ResolverVersion { get; set; } = string.Empty;

        [JsonPropertyName("groupStatus")]
        public string GroupStatus { get; set; } = string.Empty;

        [JsonPropertyName("deviceAPI_isGroup")]
        public bool DeviceAPI_isGroup { get; set; }

        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("clientID")]
        public string ClientID { get; set; } = string.Empty;

        [JsonPropertyName("productID")]
        public int ProductID { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("availability")]
        public string Availability { get; set; } = string.Empty;

        [JsonPropertyName("spotifyError")]
        public int SpotifyError { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("statusString")]
        public string StatusString { get; set; } = string.Empty;
    }
}
