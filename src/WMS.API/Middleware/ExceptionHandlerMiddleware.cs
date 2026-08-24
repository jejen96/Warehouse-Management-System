using System.Net;
using System.Text.Json;
using WMS.Application.Common;

namespace WMS.API.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next; _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, ApiResponse<object>.Fail(ex.Message)),
            ValidationException ex => (HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Validation failed.", ex.Errors)),
            BusinessException ex => (HttpStatusCode.BadRequest, ApiResponse<object>.Fail(ex.Message)),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ApiResponse<object>.Fail("Unauthorized.")),
            _ => (HttpStatusCode.InternalServerError, ApiResponse<object>.Fail("An unexpected error occurred."))
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
