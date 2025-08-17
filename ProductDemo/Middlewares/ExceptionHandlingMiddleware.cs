using ProductDemo.Exceptions;
using ProductDemo.Helpers;
using System.Net;
using System.Text.Json;

namespace ProductDemo.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Continue pipeline
        }
        catch (AppException ex)
        {
            _logger.LogError(ex, "Handled AppException");

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.FailResponse(
                ex.Errors ?? new List<string> { ex.Message },
                ex.Message
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.FailResponse("Internal server error.", "An unexpected error occurred. Please try again later.");

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}