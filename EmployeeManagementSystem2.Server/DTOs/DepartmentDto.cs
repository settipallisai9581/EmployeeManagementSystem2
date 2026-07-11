using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem2.Server.DTOs;

public record DepartmentDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    int EmployeeCount
);

public record CreateDepartmentRequest(
    [Required][MaxLength(100)] string Name,
    string? Description
);

public record UpdateDepartmentRequest(
    [Required][MaxLength(100)] string Name,
    string? Description,
    bool IsActive
);
