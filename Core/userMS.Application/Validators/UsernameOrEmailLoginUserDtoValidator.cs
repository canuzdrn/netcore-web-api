using FluentValidation;
using userMS.Application.DTOs;

namespace userMS.Application.Validators
{
    public class UsernameOrEmailLoginUserDtoValidator : AbstractValidator<UsernameOrEmailLoginUserDto>
    {
        public UsernameOrEmailLoginUserDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto.Identifier)
                .NotEmpty().WithMessage("Username / Email is required")
                .MaximumLength(25).WithMessage("Username / Email cannot exceed 320 characters"); // smtp char limit

            RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}
