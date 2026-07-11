using EmployeeManagementSystem2.Server.DTOs;
using EmployeeManagementSystem2.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(id, cancellationToken);
        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleService.CreateAsync(request, cancellationToken);
        if (role == null)
        {
            return BadRequest(new { message = "Role name already exists" });
        }

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> Update(int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleService.UpdateAsync(id, request, cancellationToken);
        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _roleService.DeleteAsync(id, cancellationToken);
        if (!result)
        {
            return BadRequest(new { message = "Cannot delete role with existing employees" });
        }

        return NoContent();
    }
}
