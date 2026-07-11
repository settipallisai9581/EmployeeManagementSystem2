using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem2.Server.DTOs;

public record RegisterRequest(
    [Required][MinLength(3)] string Username,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required] string FirstName,
    [Required] string LastName,
    [Required] int DepartmentId,
    [Required] int RoleId
);
