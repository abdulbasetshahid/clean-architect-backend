using EShop.Application.Exceptions;

namespace EShop.Api;

public sealed class GlobalExceptionHandler(
    RequestDelegate next,
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        logger.LogError(exception, "Exception handled by global middleware: {Message}", exception.Message);

        if (httpContext.Response.HasStarted)
        {
            logger.LogWarning(
                "The response has already started, so the global exception middleware cannot write a problem response.");
            return;
        }

        var problemDetails = CreateProblemDetails(httpContext, exception);

        httpContext.Response.Clear();
        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails);
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
            UnauthorizedException unauthorized => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = unauthorized.Message,
                Instance = httpContext.Request.Path
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
