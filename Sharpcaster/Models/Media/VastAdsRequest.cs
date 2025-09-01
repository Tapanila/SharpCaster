using System.Text.Json.Serialization;

namespace Sharpcaster.Models.Media
{
    /// <summary>
    /// Configuration for VAST (Video Ad Serving Template) ad requests
    /// </summary>
    public class VastAdsRequest
    {
        /// <summary>
        /// Specifies a VAST document to be used as the ads response 
        /// instead of making a request via an ad tag URL
        /// </summary>
        [JsonPropertyName("adsResponse")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AdsResponse { get; set; }

        /// <summary>
        /// URL for VAST file
        /// </summary>
        [JsonPropertyName("adTagUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AdTagUrl { get; set; }
    }
}