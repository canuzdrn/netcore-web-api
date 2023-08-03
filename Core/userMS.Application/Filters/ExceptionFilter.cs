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
            if (context.Exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(notFoundException.Message);
                context.ExceptionHandled = true;
            }
            else if (context.Exception is BadRequestException badRequestException)
            {
                context.Result = new BadRequestObjectResult(badRequestException.Message);
                context.ExceptionHandled = true;
            }
            else
            {
                // for other unhandled exceptions, return a default error response
                context.Result = new ObjectResult("An unexpected error occurred !")
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
