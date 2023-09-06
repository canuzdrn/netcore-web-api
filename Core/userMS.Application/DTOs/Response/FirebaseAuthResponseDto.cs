namespace userMS.Application.DTOs
{
    public class FirebaseAuthResponseDto
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
