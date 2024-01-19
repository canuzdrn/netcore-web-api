using FluentValidation;
using userMS.Application.DTOs;

namespace userMS.Application.Validators
{
    public class PhoneLoginUserDtoValidator : AbstractValidator<PhoneLoginUserDto>
    {
        public PhoneLoginUserDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Invalid phone number");

            RuleFor(dto => dto.Password)
                //.NotNull()
                .NotEmpty()
                .WithMessage("Password cannot be empty");
        }
    }
}
