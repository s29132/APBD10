namespace WebApplication4.Middlewares;


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
            throw; // Re-throw the exception after handling
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log error to file
        _logger.LogError(exception, "An unhandled exception occurred.");

        // Optionally, you can write a response to the client
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Internal Server Error" });
        await context.Response.WriteAsync(result);
    }
}
