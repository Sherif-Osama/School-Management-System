using Microsoft.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace School.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Forbidden access attempt on {Path} by {Username}.",
                        context.Request.Path, context.User.FindFirstValue(ClaimTypes.Name));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt on {Path}.", context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Unauthorized,
                    ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (SqlException ex) when (ex.Number == 547) // 547 represents Foreign key violation
            {
                await HandleExceptionAsync(context,
                    HttpStatusCode.Conflict,
                    "Cannot delete this record because it is referenced by other records.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred on {Path}.", context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}