using EmployeeManagementSystem2.Server.DTOs;

namespace EmployeeManagementSystem2.Server.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DepartmentDto?> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
