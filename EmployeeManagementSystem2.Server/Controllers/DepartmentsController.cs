using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await _departmentService.CreateAsync(request, cancellationToken);
        if (department == null)
        {
            return BadRequest(new { message = "Department name already exists" });
        }

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentDto>> Update(int id, [FromBody] UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await _departmentService.UpdateAsync(id, request, cancellationToken);
        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.DeleteAsync(id, cancellationToken);
        if (!result)
        {
            return BadRequest(new { message = "Cannot delete department with existing employees" });
        }

        return NoContent();
    }
}
