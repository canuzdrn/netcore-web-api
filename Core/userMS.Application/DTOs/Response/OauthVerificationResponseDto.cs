using Newtonsoft.Json;
namespace userMS.Application.DTOs.Response
{
    public class OauthVerificationResponseDto
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; }

        [JsonProperty("providerId")]
        public string ProviderId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("oauthAccessToken")]
        public string OauthAccessToken { get; set; }
    }
}
