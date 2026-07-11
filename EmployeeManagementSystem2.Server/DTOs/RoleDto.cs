using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem2.Server.DTOs;

public record RoleDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    int EmployeeCount
);

public record CreateRoleRequest(
    [Required][MaxLength(100)] string Name,
    string? Description
);

public record UpdateRoleRequest(
    [Required][MaxLength(100)] string Name,
    string? Description,
    bool IsActive
);
