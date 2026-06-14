using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Abstraction.Exceptions;
using System.Net;
using System.Text.Json;

namespace Presentation.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Handled application error: {Message}", ex.Message);
                await WriteErrorResponseAsync(context, ex.StatusCode, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument");
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation");
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.Conflict, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access");
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request timed out");
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.GatewayTimeout, "The request timed out while fetching content.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "External HTTP request failed");
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.BadGateway, "Failed to reach the target URL or external service.");
            }
            // --- NEW: Added specific handling for media/format issues if thrown directly ---
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported media or file format encountered");
                await WriteErrorResponseAsync(context, StatusCodes.Status415UnsupportedMediaType, ex.Message);
            }
            // -------------------------------------------------------------------------------
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                var message = _environment.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred.";
                await WriteErrorResponseAsync(context, (int)HttpStatusCode.InternalServerError, message);
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Detail = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }

        private static string GetTitle(int statusCode) => statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            409 => "Conflict",
            413 => "Payload Too Large",
            415 => "Unsupported Media Type",
            422 => "Unprocessable Entity",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            _ => "Error"
        };
    }
}