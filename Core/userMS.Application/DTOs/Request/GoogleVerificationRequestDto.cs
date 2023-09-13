using Newtonsoft.Json;

namespace userMS.Application.DTOs.Request
{
    public class GoogleVerificationRequestDto
    {
        [JsonProperty("requestUri")]
        public string RequestUri { get; set; }
        [JsonProperty("postBody")]
        public string PostBody { get; set; }

        public bool ReturnSecureToken = true;

        public bool ReturnIdpCredential = true;
    }
}
