namespace userMS.Application.DTOs
{
    public class UsernameOrEmailLoginUserDto
    {
        public string Identifier { get; set; } // username or email

        public string Password { get; set; }
    }
}
