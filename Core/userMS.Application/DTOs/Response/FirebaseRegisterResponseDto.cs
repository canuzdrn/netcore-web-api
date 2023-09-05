namespace userMS.Application.DTOs
{
    public class FirebaseRegisterResponseDto
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
