using EShop.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Api;

public sealed class GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = CreateProblemDetails(httpContext, exception);
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext httpContext, Exception exception)
    {
        return exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = notFound.Message,
                Instance = httpContext.Request.Path
            },
            BadRequestException badRequest => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = badRequest.Message,
                Instance = httpContext.Request.Path
            },
            ValidationException validation => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = string.Join("; ", validation.ValidationErrors),
                Instance = httpContext.Request.Path,
                Extensions = { ["errors"] = validation.ValidationErrors }
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request.",
                Instance = httpContext.Request.Path
            }
        };
    }
}
