
using System.Text.Json.Serialization;

namespace Jewelry_Model.Payload.Request.Authentication
{
    public class TokenExchangeRequest
    {
    //    [JsonPropertyName("grant_type")]
    //    public string GrantType { get; set; } = "authorization_code";
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        //[JsonPropertyName("redirect_url")]
        //public string RedirectUri { get; set; }
    }
}
