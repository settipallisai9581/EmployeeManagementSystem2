namespace EmployeeManagementSystem2.Server.Models;

public class Employee
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhotoPath { get; set; }
    public decimal? Salary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Legacy schema compatibility columns in existing EmployeeDb table.
    public string LegacyDepartment { get; set; } = string.Empty;
    public string LegacyPosition { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int? UserId { get; set; }
    public User? User { get; set; }
}
