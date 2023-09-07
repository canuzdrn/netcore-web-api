using userMS.Application.Enums;

namespace userMS.Application.DTOs.Request
{
    public class OtpVerificationRequestDto
    {
        public Guid TransactionId { get; set; }

        public int Otp { get; set; }

        public VerificationMethods VerificationMethod { get; set; }
    }
}
