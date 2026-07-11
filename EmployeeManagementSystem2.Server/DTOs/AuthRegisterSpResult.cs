namespace EmployeeManagementSystem2.Server.DTOs;

public class AuthRegisterSpResult
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
}
