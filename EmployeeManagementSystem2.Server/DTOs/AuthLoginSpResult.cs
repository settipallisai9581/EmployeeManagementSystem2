namespace EmployeeManagementSystem2.Server.DTOs;

public class AuthLoginSpResult
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
}
