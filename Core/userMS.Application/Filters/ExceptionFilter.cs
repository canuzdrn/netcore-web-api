using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using userMS.Domain.Exceptions;

namespace userMS.Application.Filters
{
    // global exception filter enable us to handle the unhandled exceptions
    // during the processing of HTTP requests
    
    // by creating a global filter we can handle requests in a centralized manner
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case NotFoundException exception:
                    context.Result = new NotFoundObjectResult(exception.Message);
                    context.ExceptionHandled = true;
                    break;

                case BadRequestException exception:
                    context.Result = new BadRequestObjectResult(exception.Message);
                    context.ExceptionHandled = true;
                    break;

                case DuplicateEntityException exception:
                    context.Result = new ConflictObjectResult(exception.Message);
                    context.ExceptionHandled = true;
                    break;

                default:
                    context.Result = new ObjectResult("An unexpected error occurred !")
                    {
                        StatusCode = 500
                    };
                    context.ExceptionHandled = true;
                    break;
            }
        }
    }
}
