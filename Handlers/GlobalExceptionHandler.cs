using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using project_z_backend.Share;

namespace project_z_backend.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        //  Log error out console:
        _logger.LogError(
            exception,
            "An error has ocurr: {message}", exception.Message
        );

        // check error type to response matching error
        var response = exception switch
        {
            BadHttpRequestException => ApiResponse.ErrorResponse(
                "Invalid request format",
                new List<string> { "The request body is not valid" },
                400),
            JsonException => ApiResponse.ErrorResponse(
                "Invalid JSON format",
                new List<string> { "The request body contains invalid JSON" },
                400),
            UnauthorizedAccessException => ApiResponse.ErrorResponse(
                "Unauthorized access",
                new List<string> { "You are not authorized to access this resource" },
                401),
            ArgumentException argEx => ApiResponse.ErrorResponse(
                "Invalid argument",
                new List<string> { argEx.Message },
                400),
            ValidationException valEx => ApiResponse.ErrorResponse(
                "Request data is not valid",
                valEx.Errors.Select(e => e.ErrorMessage).ToList(),
                400),
            _ => ApiResponse.ErrorResponse(
                "An unexpected error occurred",
                new List<string> { "Internal server error" },
                500)
        };

        // setting to response
        httpContext.Response.StatusCode = response.StatusCode ?? 500;   
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
