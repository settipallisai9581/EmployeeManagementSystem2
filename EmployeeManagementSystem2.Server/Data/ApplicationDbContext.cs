using EmployeeManagementSystem2.Server.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem2.Server.DTOs;

namespace EmployeeManagementSystem2.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AuthLoginSpResult> AuthLoginSpResults { get; set; }
    public DbSet<AuthRegisterSpResult> AuthRegisterSpResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.LegacyDepartment)
                .HasColumnName("Department")
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(e => e.LegacyPosition)
                .HasColumnName("Position")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(d => d.Name).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<AuthLoginSpResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<AuthRegisterSpResult>().HasNoKey().ToView(null);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT", Description = "Information Technology", CreatedDate = DateTime.UtcNow },
            new Department { Id = 2, Name = "HR", Description = "Human Resources", CreatedDate = DateTime.UtcNow },
            new Department { Id = 3, Name = "Finance", Description = "Finance Department", CreatedDate = DateTime.UtcNow },
            new Department { Id = 4, Name = "Sales", Description = "Sales Department", CreatedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "System Administrator", CreatedDate = DateTime.UtcNow },
            new Role { Id = 2, Name = "Manager", Description = "Department Manager", CreatedDate = DateTime.UtcNow },
            new Role { Id = 3, Name = "Employee", Description = "Regular Employee", CreatedDate = DateTime.UtcNow },
            new Role { Id = 4, Name = "Intern", Description = "Intern Employee", CreatedDate = DateTime.UtcNow }
        );
    }
}
