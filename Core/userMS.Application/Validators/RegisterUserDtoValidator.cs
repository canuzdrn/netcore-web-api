using FluentValidation;
using System.Text.RegularExpressions;
using userMS.Application.DTOs;

namespace userMS.Application.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(25).WithMessage("Username cannot exceed 25 characters")
                .MinimumLength(5).WithMessage("Username cannot be shorter than 5 characters")
                .Must(UserName => !Regex.IsMatch(UserName, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"))
                .WithMessage("Username cannot be in email format.");

            RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(6).WithMessage("Your password length must be at least 6 characters")
                .MaximumLength(30).WithMessage("Your password length must not exceed 30 characters")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number");

            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(dto => dto.PhoneNo)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Invalid phone number");
        }
    }
}
