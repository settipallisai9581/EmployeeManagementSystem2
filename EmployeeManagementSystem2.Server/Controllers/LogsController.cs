using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem2.Server.Controllers;

[ApiController]
[Route("api/logs")]
[AllowAnonymous]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogger<LogsController> logger)
    {
        _logger = logger;
    }

    [HttpPost("client")]
    public IActionResult LogClientEvent([FromBody] JsonElement request)
    {
        static string? ReadString(JsonElement element, string name)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (element.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                return prop.GetString();
            }

            return null;
        }

        var levelRaw = ReadString(request, "level") ?? "Info";
        var message = ReadString(request, "message") ?? "Client event received without message";
        var sessionId = ReadString(request, "sessionId") ?? "unknown";
        var route = ReadString(request, "route") ?? "unknown";
        var loggedAtUtc = ReadString(request, "loggedAtUtc") ?? DateTimeOffset.UtcNow.ToString("O");

        object? metadata = null;
        if (request.ValueKind == JsonValueKind.Object && request.TryGetProperty("metadata", out var metadataProp))
        {
            metadata = metadataProp.ValueKind == JsonValueKind.Null
                ? null
                : metadataProp.GetRawText();
        }

        var level = levelRaw.Trim().ToLowerInvariant();
        var template = "ClientLog Level={Level} SessionId={SessionId} Route={Route} LoggedAtUtc={LoggedAtUtc} Message={Message} Metadata={Metadata}";

        if (level == "error")
        {
            _logger.LogError(template, levelRaw, sessionId, route, loggedAtUtc, message, metadata);
        }
        else if (level == "warn" || level == "warning")
        {
            _logger.LogWarning(template, levelRaw, sessionId, route, loggedAtUtc, message, metadata);
        }
        else if (level == "debug" || level == "trace")
        {
            _logger.LogDebug(template, levelRaw, sessionId, route, loggedAtUtc, message, metadata);
        }
        else
        {
            _logger.LogInformation(template, levelRaw, sessionId, route, loggedAtUtc, message, metadata);
        }

        return Ok(new { accepted = true });
    }
}