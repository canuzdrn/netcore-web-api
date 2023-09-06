using System.Text.Json.Serialization;

namespace userMS.Application.DTOs.Response
{
    public class FirebasePhoneVerificationResponseDto
    {
        [JsonPropertyName("sessionInfo")]
        public string SessionInfo { get; set; }
    }
}
