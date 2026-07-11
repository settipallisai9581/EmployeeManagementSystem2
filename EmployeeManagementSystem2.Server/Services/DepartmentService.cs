using EmployeeManagementSystem2.Server.Data;
using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem2.Server.Services;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;

    public DepartmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .Select(d => MapToDto(d))
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        return department == null ? null : MapToDto(department);
    }

    public async Task<DepartmentDto?> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _context.Departments.AnyAsync(d => d.Name == request.Name, cancellationToken))
        {
            return null;
        }

        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(department.Id, cancellationToken);
    }

    public async Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var department = await _context.Departments.FindAsync([id], cancellationToken);
        if (department == null)
        {
            return null;
        }

        if (await _context.Departments.AnyAsync(d => d.Name == request.Name && d.Id != id, cancellationToken))
        {
            return null;
        }

        department.Name = request.Name;
        department.Description = request.Description;
        department.IsActive = request.IsActive;
        department.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(department.Id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department == null)
        {
            return false;
        }

        if (department.Employees.Any())
        {
            return false;
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto(
            Id: department.Id,
            Name: department.Name,
            Description: department.Description,
            IsActive: department.IsActive,
            EmployeeCount: department.Employees.Count
        );
    }
}
