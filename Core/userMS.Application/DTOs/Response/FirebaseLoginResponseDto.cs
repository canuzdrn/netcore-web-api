namespace userMS.Application.DTOs.Response
{
    public class FirebaseLoginResponseDto
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
