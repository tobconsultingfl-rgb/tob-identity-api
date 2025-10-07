using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;
using TOB.Identity.Services;
using System.Linq;

namespace TOB.Identity.API.Controllers;

[Route("/roles")]
[ApiController]
public class RolesController : BaseController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a new role",
        OperationId = "CreateRole",
        Tags = new[] { "Roles" }
    )]
    [ProducesResponseType(typeof(RoleDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    [Produces("application/json")]

    [Authorize]
    public async Task<IActionResult> CreateRoleAsync([FromBody] RoleDto roleDTO)
    {
        var currentUserId = new Guid(CurrentUserId);
        var response = await _roleService.CreateRoleAsync(roleDTO, currentUserId);

        return Created(string.Empty, response);
    }
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get All Roles",
        OperationId = "GetAllRolesAsync",
        Tags = new[] { "Roles" }
    )]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        var roles = await _roleService.GetAllRolesAsync();

        if (!roles.Any())
        {
            return NotFound();
        }

        return Ok(roles);
    }

    [Authorize]
    [HttpPut("{roleId}", Name = "Update Role")]
    [SwaggerOperation(
                Summary = "Updates a role",
                OperationId = "UpdateRoleAsync",
                Tags = new[] { "Roles" }
            )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateRoleAsync(Guid roleId, [FromBody] RoleDto roleDto)
    {
        var roles = await _roleService.GetAllRolesAsync();

        if (!roles.Where(m => m.RoleId == roleId).Any())
        {
            return NotFound();
        }

        if (roleDto.RoleId != roleId)
        {
            return BadRequest();
        }

        var currentUserId = new Guid(CurrentUserId);

        var results = await _roleService.UpdateRoleAsync(roleDto, currentUserId);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{roleId}", Name = "Delete (deactivates) Role")]
    [SwaggerOperation(
         Summary = "Deletes (deactivates) a role",
         OperationId = "DeleteRoleAsync",
         Tags = new[] { "Roles" }
     )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteRoleAsync(Guid roleId)
    {
        var roles = await _roleService.GetAllRolesAsync();

        if (!roles.Where(m => m.RoleId == roleId).Any())
        {
            return NotFound();
        }

        var currentUserId = new Guid(CurrentUserId);

        var result = await _roleService.DeleteRoleAsync(roleId, currentUserId);

        return NoContent();
    }
}
