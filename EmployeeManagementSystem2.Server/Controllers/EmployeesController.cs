using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> Search([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        var employees = await _employeeService.SearchAsync(searchTerm, cancellationToken);
        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (request.DepartmentId <= 0 || request.RoleId <= 0)
        {
            return BadRequest(new { message = "Please select a valid Department and Role." });
        }

        if (request.Salary.HasValue && request.Salary.Value < 1)
        {
            return BadRequest(new { message = "Salary must be greater than or equal to 1." });
        }

        try
        {
            var employee = await _employeeService.CreateAsync(request, cancellationToken);
            if (employee == null)
            {
                return BadRequest(new { message = "Email already exists." });
            }

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (request.DepartmentId <= 0 || request.RoleId <= 0)
        {
            return BadRequest(new { message = "Please select a valid Department and Role." });
        }

        if (request.Salary.HasValue && request.Salary.Value < 1)
        {
            return BadRequest(new { message = "Salary must be greater than or equal to 1." });
        }

        try
        {
            var employee = await _employeeService.UpdateAsync(id, request, cancellationToken);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            return Ok(employee);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.DeleteAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id}/photo")]
    public async Task<ActionResult<string>> UploadPhoto(int id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded" });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { message = "Invalid file type. Only JPG, PNG, and GIF are allowed" });
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { message = "File size must be less than 5MB" });
        }

        var photoPath = await _employeeService.UploadPhotoAsync(id, file, cancellationToken);
        if (photoPath == null)
        {
            return NotFound();
        }

        return Ok(new { photoPath });
    }

    [HttpGet("report")]
    public async Task<ActionResult<EmployeeReportDto>> GetReport(CancellationToken cancellationToken)
    {
        var report = await _employeeService.GetReportAsync(cancellationToken);
        return Ok(report);
    }
}
