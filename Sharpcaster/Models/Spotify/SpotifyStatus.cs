using System.Runtime.Serialization;

namespace Sharpcaster.Models.Spotify
{
    [DataContract]
    public class SpotifyStatus
    {
        [DataMember(Name = "version")]
        public string Version { get; set; }
        [DataMember(Name = "publicKey")]
        public string PublicKey { get; set; }
        [DataMember(Name = "remoteName")]
        public string RemoteName { get; set; }
        [DataMember(Name = "deviceType")]
        public string DeviceType { get; set; }
        [DataMember(Name = "brandDisplayName")]
        public string BrandDisplayName { get; set; }
        [DataMember(Name = "modelDisplayName")]
        public string ModelDisplayName { get; set; }
        [DataMember(Name = "libraryVersion")]
        public string LibraryVersion { get; set; }
        [DataMember(Name = "resolverVersion")]
        public string ResolverVersion { get; set; }
        [DataMember(Name = "groupStatus")]
        public string GroupStatus { get; set; }
        [DataMember(Name = "deviceAPI_isGroup")]
        public bool DeviceAPI_isGroup { get; set; }
        [DataMember(Name = "tokenType")]
        public string TokenType { get; set; }
        [DataMember(Name = "clientID")]
        public string ClientID { get; set; }
        [DataMember(Name = "productID")]
        public int ProductID { get; set; }
        [DataMember(Name = "scope")]
        public string Scope { get; set; }
        [DataMember(Name = "availability")]
        public string Availability { get; set; }
        [DataMember(Name = "spotifyError")]
        public int SpotifyError { get; set; }
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name = "statusString")]
        public string StatusString { get; set; }
    }
}
