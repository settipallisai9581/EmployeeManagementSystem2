using EmployeeManagementSystem2.Server.DTOs;

namespace EmployeeManagementSystem2.Server.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleDto?> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<RoleDto?> UpdateAsync(int id, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
