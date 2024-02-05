using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FC.Codeflix.Catalog.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _env;
    public ApiGlobalExceptionFilter(IHostEnvironment env) 
        => _env = env;

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exceptionInContext = context.Exception;

        if (_env.IsDevelopment())
            details.Extensions.Add("StackTrace", exceptionInContext.StackTrace);

        switch (exceptionInContext)
        {
            case EntityValidationException entityValidationException:
                details.Title = "One or more validation errors ocurred";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnprocessableEntity";
                details.Detail = entityValidationException.Message;
                break;
            case NotFoundException notFoundException:
                details.Type = "NotFound";
                details.Detail = notFoundException.Message;
                details.Status = StatusCodes.Status404NotFound;
                details.Title = "Not Found";
                break;
            case RelatedAggregateException relatedAggregateException:
                details.Title = "Invalid Related Aggregate";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "RelatedAggregate";
                details.Detail = relatedAggregateException.Message;
                break;
            default:
                details.Title = "An unexpected error ocurred";
                details.Status = StatusCodes.Status500InternalServerError;
                details.Type = "UnexpectedError";
                details.Detail = exceptionInContext.Message;
                break;
        }


        context.HttpContext.Response.StatusCode = (int)details.Status;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}
