namespace userMS.Application.DTOs.Response
{
    public class OtpObjectStored
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Otp { get; set; }
    }
}
