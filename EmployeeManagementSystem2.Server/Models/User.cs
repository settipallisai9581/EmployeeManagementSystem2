namespace EmployeeManagementSystem2.Server.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee? Employee { get; set; }
}
