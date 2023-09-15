using Newtonsoft.Json;

namespace userMS.Application.DTOs.Request
{
    public class OauthVerificationRequestDto
    {
        [JsonProperty("requestUri")]
        public string RequestUri { get; set; }
        [JsonProperty("postBody")]
        public string PostBody { get; set; }

        public bool ReturnSecureToken { get; set; } = true;

        public bool ReturnIdpCredential { get; set; } = true;
    }
}
