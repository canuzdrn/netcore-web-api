namespace userMS.Application.DTOs.Request
{
    public class UserRegisterMailRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
