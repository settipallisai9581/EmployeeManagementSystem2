using EmployeeManagementSystem2.Server.Data;
using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem2.Server.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Employees)
            .OrderBy(r => r.Name)
            .Select(r => MapToDto(r))
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.Employees)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto?> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _context.Roles.AnyAsync(r => r.Name == request.Name, cancellationToken))
        {
            return null;
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(role.Id, cancellationToken);
    }

    public async Task<RoleDto?> UpdateAsync(int id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var role = await _context.Roles.FindAsync([id], cancellationToken);
        if (role == null)
        {
            return null;
        }

        if (await _context.Roles.AnyAsync(r => r.Name == request.Name && r.Id != id, cancellationToken))
        {
            return null;
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;
        role.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(role.Id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.Employees)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
        {
            return false;
        }

        if (role.Employees.Any())
        {
            return false;
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto(
            Id: role.Id,
            Name: role.Name,
            Description: role.Description,
            IsActive: role.IsActive,
            EmployeeCount: role.Employees.Count
        );
    }
}
