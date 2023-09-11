namespace userMS.Application.DTOs.Response
{
    public class OtpObjectStored
    {
        public string TransactionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Otp { get; set; }
    }
}
