using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem2.Server.DTOs;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password
);
