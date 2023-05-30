using System.Net;
using System.Text.Json;
using Infrastructure.Exceptions;

namespace Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            var userNotFoundException = new UserNotFoundException();
            var textPayload = "";
            
            if (ex.GetBaseException().GetType() == typeof(UserNotFoundException))
            {
                _logger.LogError(ex, "User Not Found Exception.");
                textPayload = "User Not Found";
            }
            
            var errorResponse = new ErrorResponse
            {
                Message = textPayload,
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            var errorJson = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(errorJson);
        }
    }
}

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}

public class ErrorResponse
{
    public string Message { get; set; }
    // Include additional properties as needed
}