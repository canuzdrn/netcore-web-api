using System.Text.Json.Serialization;

namespace userMS.Application.DTOs
{
    public class FirebaseError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
