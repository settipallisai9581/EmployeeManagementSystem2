using EmployeeManagementSystem2.Server.DTOs;

namespace EmployeeManagementSystem2.Server.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<string?> UploadPhotoAsync(int id, IFormFile file, CancellationToken cancellationToken = default);
    Task<EmployeeReportDto> GetReportAsync(CancellationToken cancellationToken = default);
}
