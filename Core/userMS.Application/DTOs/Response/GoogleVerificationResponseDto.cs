using Newtonsoft.Json;

namespace userMS.Application.DTOs.Response
{
    public class GoogleVerificationResponseDto
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; }
        [JsonProperty("oauthAccessToken")]
        public string OauthAccessToken { get; set; }
        [JsonProperty("oauthExpireIn")]
        public int OauthExpireIn { get; set; }
    }
}
