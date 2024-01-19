namespace userMS.Application.DTOs.Request
{
    public class ExternalLoginEmailRequestDto
    {
        public string Username { get; set; }
        public string Provider { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
    }
}
