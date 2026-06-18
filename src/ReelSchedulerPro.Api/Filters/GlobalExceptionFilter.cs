using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReelSchedulerPro.Shared.Exceptions;
using Serilog;

namespace ReelSchedulerPro.Api.Filters;

/// <summary>
/// Global exception filter for handling application exceptions
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = new { error = exception.Message };
        var statusCode = StatusCodes.Status500InternalServerError;

        if (exception is ValidationException validationException)
        {
            statusCode = validationException.StatusCode;
            response = new { error = exception.Message, errors = validationException.Errors };
        }
        else if (exception is UnauthorizedException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
        }
        else if (exception is ForbiddenException)
        {
            statusCode = StatusCodes.Status403Forbidden;
        }
        else if (exception is NotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
