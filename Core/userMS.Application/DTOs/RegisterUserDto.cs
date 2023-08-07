namespace userMS.Application.DTOs
{
    public class RegisterUserDto
    {
        public string Id { get => Guid.NewGuid().ToString(); }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNo { get; set; }
    }
}
