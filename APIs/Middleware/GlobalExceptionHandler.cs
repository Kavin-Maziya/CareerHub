using APIs.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            JobNotFoundException => StatusCodes.Status404NotFound,
            ApplicationNotFoundException => StatusCodes.Status404NotFound,
            DuplicateJobException => StatusCodes.Status409Conflict,
            DuplicateApplicationException => StatusCodes.Status409Conflict,
            ListingClosedException => StatusCodes.Status400BadRequest,
            UnauthorizedWithdrawalException => StatusCodes.Status403Forbidden,
            UnauthorizedCompanyException => StatusCodes.Status403Forbidden,
            InvalidStatusTransitionException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status404NotFound => "Resource Not Found",
        StatusCodes.Status409Conflict => "Resource Conflict",
        StatusCodes.Status400BadRequest => "Bad Request",
        _ => "Internal Server Error"
    };
}