using userMS.Application.Enums;

namespace userMS.Application.DTOs.Request
{
    public class OtpVerificationRequestDto
    {
        public string TransactionId { get; set; }

        public string Otp { get; set; }

        public VerificationMethods VerificationMethod { get; set; }
    }
}
