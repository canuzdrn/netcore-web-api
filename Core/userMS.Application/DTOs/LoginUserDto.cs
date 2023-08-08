namespace userMS.Application.DTOs
{
    public class LoginUserDto
    {
        public string Identifier { get; set; } // username or email

        public string Password { get; set; }
    }
}
