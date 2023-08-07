﻿using FluentValidation;
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

            RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(6).WithMessage("Your password length must be at least 6.")
                .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");

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
