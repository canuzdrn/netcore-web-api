namespace userMS.Application.DTOs.Request
{
    public class EmailSendRequestDto
    {
        public string? Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
    }
}
