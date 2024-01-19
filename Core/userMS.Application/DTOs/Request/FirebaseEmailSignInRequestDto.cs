namespace userMS.Application.DTOs
{
    public class FirebaseEmailSignInRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool ReturnSecureToken => true;
    }
}
