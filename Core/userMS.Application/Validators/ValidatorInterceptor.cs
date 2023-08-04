using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace userMS.Application.Validators
{
    // interceptor serves as a way to centralize the handling of validation errors and
    // apply consistent logic to format and return the validation error responses
    public class ValidatorInterceptor : IValidatorInterceptor
    {
        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }

        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            var validationFailures = result.Errors
                .Select(error => 
                new ValidationFailure(error.PropertyName, error.ErrorMessage));

            return new ValidationResult(validationFailures);
        }
    }
}
