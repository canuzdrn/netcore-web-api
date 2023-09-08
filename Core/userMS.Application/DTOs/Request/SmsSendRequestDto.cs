namespace userMS.Application.DTOs.Request
{
    public class SmsSendRequestDto
    {
        public string To { get; set; }
        public string Body = string.Empty;
    }
}
