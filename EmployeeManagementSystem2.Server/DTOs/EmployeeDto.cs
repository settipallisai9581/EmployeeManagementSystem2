using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem2.Server.DTOs;

public record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateTime? DateOfBirth,
    DateTime HireDate,
    string? Address,
    string? City,
    string? State,
    string? ZipCode,
    string? PhotoPath,
    decimal? Salary,
    bool IsActive,
    int DepartmentId,
    string DepartmentName,
    int RoleId,
    string RoleName
);

public record CreateEmployeeRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required][EmailAddress] string Email,
    string? Phone,
    DateTime? DateOfBirth,
    DateTime HireDate,
    string? Address,
    string? City,
    string? State,
    string? ZipCode,
    decimal? Salary,
    [Required] int DepartmentId,
    [Required] int RoleId
);

public record UpdateEmployeeRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required][EmailAddress] string Email,
    string? Phone,
    DateTime? DateOfBirth,
    DateTime HireDate,
    string? Address,
    string? City,
    string? State,
    string? ZipCode,
    decimal? Salary,
    bool IsActive,
    [Required] int DepartmentId,
    [Required] int RoleId
);
