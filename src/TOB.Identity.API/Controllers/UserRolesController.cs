using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Services;
using System.Linq;

namespace TOB.Identity.API.Controllers;

[Authorize]
[Route("{userId}/roles")]
[ApiController]
public class UserRolesController : BaseController
{
    private readonly IUserRoleService _roleService;
    private IMemoryCache _memoryCache;

    public UserRolesController(IUserRoleService roleService, IMemoryCache memoryCache)
    {
        _roleService = roleService;
        _memoryCache = memoryCache;
    }


    [HttpGet]
    [SwaggerOperation(
        Summary = "Get User's Roles",
        OperationId = "GetUsersRoles",
        Tags = new[] { "User Roles" }
    )]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUsersRolesAsync(Guid userId)
    {
        if (_memoryCache.TryGetValue(userId, out IEnumerable<RoleDto> cachedUser))
        {
            return Ok(true);
        }

        var roles = await _roleService.GetRolesByUserIdAsync(userId);

        if (roles == null || !roles.ToList().Any())
        {
            return NotFound();
        }

        _memoryCache.Set(userId, roles);

        return Ok(roles);
    }

    [HttpPatch]
    [SwaggerOperation(
        Summary = "Adds a user to a role",
        OperationId = "AssignUserToRoleAsync",
        Tags = new[] { "User Roles" }
    )]
    [ProducesResponseType(typeof(IEnumerable<bool>), 200)]
    [ProducesResponseType(404)]

    public async Task<IActionResult> AssignUserToRoleAsync(CreateUserRoleRequest createUserRoleRequest)
    {
        var results = await _roleService.AssignUserToRoleAsync(createUserRoleRequest, new Guid(CurrentUserId));

        return Ok(results);
    }
}
