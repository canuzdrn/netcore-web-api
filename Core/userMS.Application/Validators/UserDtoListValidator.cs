using FluentValidation;
using userMS.Application.DTOs;

namespace userMS.Application.Validators
{
    public class UserDtoListValidator : AbstractValidator<IEnumerable<UserDto>>
    {
        public UserDtoListValidator()
        {
            RuleForEach(dto => dto).SetValidator(new UserDtoValidator());
        }
    }
}
