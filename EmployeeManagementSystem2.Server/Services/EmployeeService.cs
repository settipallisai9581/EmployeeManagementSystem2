using EmployeeManagementSystem2.Server.Data;
using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeeManagementSystem2.Server.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EmployeeService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteEmployeeReaderAsync(
            "dbo.usp_Employees_GetAll",
            null,
            cancellationToken);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var rows = await ExecuteEmployeeReaderAsync(
            "dbo.usp_Employees_GetById",
            command => command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id }),
            cancellationToken);

        return rows.FirstOrDefault();
    }

    public async Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var term = searchTerm.ToLowerInvariant();
        var employees = await GetAllAsync(cancellationToken);

        return employees.Where(e =>
            e.FirstName.ToLowerInvariant().Contains(term) ||
            e.LastName.ToLowerInvariant().Contains(term) ||
            e.Email.ToLowerInvariant().Contains(term) ||
            (!string.IsNullOrWhiteSpace(e.Phone) && e.Phone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
            e.DepartmentName.ToLowerInvariant().Contains(term) ||
            e.RoleName.ToLowerInvariant().Contains(term));
    }

    public async Task<EmployeeDto?> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var rows = await ExecuteEmployeeReaderAsync(
                "dbo.usp_Employees_Create",
                command =>
                {
                    command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = request.FirstName });
                    command.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = request.LastName });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = request.Email });
                    command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar) { Value = (object?)request.Phone ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime2) { Value = (object?)request.DateOfBirth ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.DateTime2) { Value = request.HireDate });
                    command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = (object?)request.Address ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = (object?)request.City ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@State", SqlDbType.NVarChar) { Value = (object?)request.State ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@ZipCode", SqlDbType.NVarChar) { Value = (object?)request.ZipCode ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Salary", SqlDbType.Decimal)
                    {
                        Precision = 18,
                        Scale = 2,
                        Value = (object?)request.Salary ?? DBNull.Value
                    });
                    command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = request.DepartmentId });
                    command.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = request.RoleId });
                    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = DBNull.Value });
                },
                cancellationToken);

            return rows.FirstOrDefault();
        }
        catch (SqlException ex) when (ex.Number == 51000)
        {
            return null;
        }
        catch (SqlException ex) when (ex.Number == 51001)
        {
            throw new InvalidOperationException("Invalid Department or Role selected.", ex);
        }
        catch (SqlException ex) when (ex.Number == 547 && ex.Message.Contains("CK_Employees_Salary", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Salary must be greater than or equal to 1.", ex);
        }
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var rows = await ExecuteEmployeeReaderAsync(
                "dbo.usp_Employees_Update",
                command =>
                {
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                    command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = request.FirstName });
                    command.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = request.LastName });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = request.Email });
                    command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar) { Value = (object?)request.Phone ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime2) { Value = (object?)request.DateOfBirth ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.DateTime2) { Value = request.HireDate });
                    command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = (object?)request.Address ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar) { Value = (object?)request.City ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@State", SqlDbType.NVarChar) { Value = (object?)request.State ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@ZipCode", SqlDbType.NVarChar) { Value = (object?)request.ZipCode ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Salary", SqlDbType.Decimal)
                    {
                        Precision = 18,
                        Scale = 2,
                        Value = (object?)request.Salary ?? DBNull.Value
                    });
                    command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive });
                    command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = request.DepartmentId });
                    command.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = request.RoleId });
                    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = DBNull.Value });
                },
                cancellationToken);

            return rows.FirstOrDefault();
        }
        catch (SqlException ex) when (ex.Number == 51002)
        {
            return null;
        }
        catch (SqlException ex) when (ex.Number == 51003)
        {
            throw new InvalidOperationException("Employee email already exists.", ex);
        }
        catch (SqlException ex) when (ex.Number == 51004)
        {
            throw new InvalidOperationException("Invalid Department or Role selected.", ex);
        }
        catch (SqlException ex) when (ex.Number == 547 && ex.Message.Contains("CK_Employees_Salary", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Salary must be greater than or equal to 1.", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var connectionString = _context.Database.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("dbo.usp_Employees_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var rowsDeleted = 0;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            rowsDeleted = reader["RowsDeleted"] is DBNull ? 0 : Convert.ToInt32(reader["RowsDeleted"]);
        }

        return rowsDeleted > 0;
    }

    public async Task<string?> UploadPhotoAsync(int id, IFormFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        var employee = await _context.Employees.FindAsync([id], cancellationToken);
        if (employee == null)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "photos");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        if (!string.IsNullOrEmpty(employee.PhotoPath))
        {
            var oldPhotoPath = Path.Combine(_environment.WebRootPath, employee.PhotoPath.TrimStart('/'));
            if (File.Exists(oldPhotoPath))
            {
                File.Delete(oldPhotoPath);
            }
        }

        employee.PhotoPath = $"/uploads/photos/{fileName}";
        employee.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return employee.PhotoPath;
    }

    public async Task<EmployeeReportDto> GetReportAsync(CancellationToken cancellationToken = default)
    {
        var employees = (await GetAllAsync(cancellationToken)).ToList();

        var departmentSummaries = employees
            .GroupBy(e => e.DepartmentName)
            .Select(g => new DepartmentSummary(g.Key, g.Count()))
            .ToList();

        var roleSummaries = employees
            .GroupBy(e => e.RoleName)
            .Select(g => new RoleSummary(g.Key, g.Count()))
            .ToList();

        var salaries = employees.Where(e => e.Salary.HasValue).Select(e => e.Salary!.Value).ToList();
        var averageSalary = salaries.Any() ? salaries.Average() : 0;
        var totalSalary = salaries.Sum();

        return new EmployeeReportDto(
            TotalEmployees: employees.Count,
            ActiveEmployees: employees.Count(e => e.IsActive),
            InactiveEmployees: employees.Count(e => !e.IsActive),
            DepartmentSummaries: departmentSummaries,
            RoleSummaries: roleSummaries,
            AverageSalary: averageSalary,
            TotalSalary: totalSalary
        );
    }

    private async Task<List<EmployeeDto>> ExecuteEmployeeReaderAsync(
        string procedureName,
        Action<SqlCommand>? parameterBuilder,
        CancellationToken cancellationToken)
    {
        var connectionString = _context.Database.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        parameterBuilder?.Invoke(command);

        var results = new List<EmployeeDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(MapReaderToDto(reader));
        }

        return results;
    }

    private static EmployeeDto MapReaderToDto(SqlDataReader reader)
    {
        static string SafeString(SqlDataReader r, string columnName)
            => r[columnName] is DBNull ? string.Empty : Convert.ToString(r[columnName]) ?? string.Empty;

        static string? SafeNullableString(SqlDataReader r, string columnName)
            => r[columnName] is DBNull ? null : Convert.ToString(r[columnName]);

        static DateTime? SafeNullableDateTime(SqlDataReader r, string columnName)
            => r[columnName] is DBNull ? null : Convert.ToDateTime(r[columnName]);

        static decimal? SafeNullableDecimal(SqlDataReader r, string columnName)
            => r[columnName] is DBNull ? null : Convert.ToDecimal(r[columnName]);

        return new EmployeeDto(
            Id: Convert.ToInt32(reader["Id"]),
            FirstName: SafeString(reader, "FirstName"),
            LastName: SafeString(reader, "LastName"),
            Email: SafeString(reader, "Email"),
            Phone: SafeNullableString(reader, "Phone"),
            DateOfBirth: SafeNullableDateTime(reader, "DateOfBirth"),
            HireDate: Convert.ToDateTime(reader["HireDate"]),
            Address: SafeNullableString(reader, "Address"),
            City: SafeNullableString(reader, "City"),
            State: SafeNullableString(reader, "State"),
            ZipCode: SafeNullableString(reader, "ZipCode"),
            PhotoPath: SafeNullableString(reader, "PhotoPath"),
            Salary: SafeNullableDecimal(reader, "Salary"),
            IsActive: reader["IsActive"] is not DBNull && Convert.ToBoolean(reader["IsActive"]),
            DepartmentId: reader["DepartmentId"] is DBNull ? 0 : Convert.ToInt32(reader["DepartmentId"]),
            DepartmentName: reader["DepartmentName"] is DBNull ? "Unknown" : SafeString(reader, "DepartmentName"),
            RoleId: reader["RoleId"] is DBNull ? 0 : Convert.ToInt32(reader["RoleId"]),
            RoleName: reader["RoleName"] is DBNull ? "Unknown" : SafeString(reader, "RoleName")
        );
    }
}
