namespace userMS.Application.DTOs
{
    public class FirebaseRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool ReturnSecureToken => true;
    }
}
