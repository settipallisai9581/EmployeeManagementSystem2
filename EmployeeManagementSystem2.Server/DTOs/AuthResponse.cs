namespace EmployeeManagementSystem2.Server.DTOs;

public record AuthResponse(
    string Token,
    string Username,
    string Email,
    int UserId,
    int? EmployeeId
);
