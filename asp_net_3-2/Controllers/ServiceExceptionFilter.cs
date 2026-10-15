using asp_net_3_2.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace asp_net_3_2.Controllers;

public sealed class ServiceExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var statusCode = context.Exception switch
        {
            EntityNotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            _ => (int?)null
        };
        if (statusCode is null)
        {
            return;
        }

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status404NotFound ? "Resource not found" : "Operation conflict",
            Detail = context.Exception.Message
        }) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}
