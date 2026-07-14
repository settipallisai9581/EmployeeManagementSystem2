using System.Diagnostics;

namespace EmployeeManagementSystem2.Server.Middleware;

public class ApiRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiRequestLoggingMiddleware> _logger;

    public ApiRequestLoggingMiddleware(RequestDelegate next, ILogger<ApiRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var startedAt = DateTimeOffset.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "API request started. TraceId={TraceId} Method={Method} Path={Path} Query={Query} User={User} StartedAtUtc={StartedAtUtc}",
            context.TraceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString.Value,
            context.User.Identity?.Name ?? "anonymous",
            startedAt);

        try
        {
            await _next(context);

            stopwatch.Stop();
            _logger.LogInformation(
                "API request completed. TraceId={TraceId} Method={Method} Path={Path} StatusCode={StatusCode} DurationMs={DurationMs}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "API request failed. TraceId={TraceId} Method={Method} Path={Path} DurationMs={DurationMs}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}