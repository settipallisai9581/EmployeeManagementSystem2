namespace EmployeeManagementSystem2.Server.DTOs;

public class ClientLogEntryRequest
{
    public string Level { get; set; } = "Info";
    public string Message { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? SessionId { get; set; }
    public object? Metadata { get; set; }
    public DateTimeOffset LoggedAtUtc { get; set; }
}