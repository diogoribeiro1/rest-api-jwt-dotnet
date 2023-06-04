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

                var statusCode = HttpStatusCode.BadRequest;
                string message;

                switch (ex.GetBaseException())
                {
                    case UserNotFoundException _:
                        _logger.LogError(ex, "User Not Found Exception.");
                        message = "User Not Found";
                        break;
                    case UsernameAlreadyExistsException _:
                        _logger.LogError(ex, "Username Already exists");
                        message = "Username Already exists";
                        break;
                    case WrongPasswordException _:
                        _logger.LogError(ex, "Wrong password");
                        message = "Wrong password";
                        break;
                    default:
                        message = "An error occurred";
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }

                var errorResponse = new ErrorResponse
                {
                    Message = message,
                };

                var errorJson = JsonSerializer.Serialize(errorResponse);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
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