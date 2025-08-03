using ProductDemo.Exceptions;
using System.Net;
using System.Text.Json;

namespace ProductDemo.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next; // This is the next middleware in the pipeline
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // proceed to next
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "Handled exception occurred");

                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "An error occurred handled by AppException.",
                    Details = ex.Message // Optional: Only show in Development
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "An unexpected error occurred. Please try again later.",
                    Details = ex.Message // Optional: Only show in Development
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

}
