using FluentValidation;
using userMS.Application.DTOs;

namespace userMS.Application.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator() {

            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto.Id)
                .NotNull().WithMessage("Id field cannot be null")
                .NotEmpty().WithMessage("Id is required")
                .Matches(@"^[A-Fa-f0-9]{8}(-[A-Fa-f0-9]{4}){4}[A-Fa-f0-9]{8}$") // Guid Regex
                .WithMessage("Invalid Id format. The Id should be in GUID format");

            RuleFor(dto => dto.UserName)
                .NotNull().WithMessage("Username cannot be null")
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(25).WithMessage("Username cannot exceed 25 characters")
                .MinimumLength(5).WithMessage("Username cannot be shorter than 5 characters");

            RuleFor(dto => dto.Email)
                .NotNull().WithMessage("Email cannot be null")
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(dto => dto.PhoneNo)
                .NotNull().WithMessage("Phone number cannot be null")
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Invalid phone number");

        }
    }
}
