using FluentValidation;
using userMS.Application.DTOs.Request;

namespace userMS.Application.Validators
{
    public class OtpVerificationRequestDtoValidator : AbstractValidator<OtpVerificationRequestDto>
    {
        public OtpVerificationRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto.TransactionId)
                .NotEmpty().WithMessage("TransactionId cannot be null or empty");

            RuleFor(dto => dto.Otp)
                .NotEmpty().WithMessage("OTP must be provided");
        }
    }
}
