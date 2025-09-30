using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models.Requests;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Services;
using System.Linq;

namespace TOB.Identity.API.Controllers;

[Authorize]
[Route("/users")]
[ApiController]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("me")]
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get The Logged by User",
        OperationId = "GetCurrentUser",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var user = await _userService.GetUserByIdAsync(new Guid(CurrentUserId));

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet]
    [Route("usernameexists/{userName}")]
    [SwaggerOperation(
        Summary = "Check for existence of a username",
        OperationId = "DoesUserNameExists",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DoesUserNameExists(string userName)
    {
        var doesExists = await _userService.DoesUsernameExistsAsync(userName);

        return Ok(doesExists);
    }

    [HttpPost()]
    [SwaggerOperation(
        Summary = "Creates a new user",
        OperationId = "CreateUserAsync",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    [Produces("application/json")]

    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest createUserRequest)
    {
        var userExists = await _userService.DoesUsernameExistsAsync(createUserRequest.Email);

        if (userExists)
        {
            var error = new Dictionary<string, string[]>();
            var errors = new List<string> { "User Already Exists" };
            error.Add("UserExists", errors.ToArray());

            var validationProblem = new ValidationProblemDetails(error);
            return BadRequest(validationProblem);
        }

        var response = await _userService.CreateUserAsync(createUserRequest, new Guid(CurrentUserId));

        return Created(string.Empty, response);
    }

    [HttpGet("{userId}", Name = "Get User By ID")]
    [SwaggerOperation(
        Summary = "Get User by ID",
        OperationId = "GetUserById",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get Users by TenantID",
        OperationId = "GetUsersByTenantID",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUsersByTenantID([FromQuery] Guid TenantId)
    {
        var users = await _userService.GetAllUsersByTenantIdAsync(TenantId);

        if (users == null || !users.ToList().Any())
        {
            return NotFound();
        }

        return Ok(users);
    }

    [HttpPut("{userId}", Name = "Update User")]
    [SwaggerOperation(
        Summary = "Updates a user",
        OperationId = "UpdateUserAsync",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateUserAsync(Guid userId, [FromBody] UpdateUserRequest updateUserRequest)
    {
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        if (updateUserRequest.UserId != userId)
        {
            return BadRequest();
        }

        var results = await _userService.UpdateUserAsync(updateUserRequest, new Guid(CurrentUserId));
        var updatedUser = await _userService.GetUserByIdAsync(userId);

        return NoContent();
    }

    [HttpDelete("{userId}", Name = "Delete (deactivates) User")]
    [SwaggerOperation(
        Summary = "Deletes (deactivates) a user",
        OperationId = "DeleteUserAsync",
        Tags = new[] { "Users" }
    )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteUserAsync(Guid userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var result = await _userService.DeleteUserAsync(userId, new Guid(CurrentUserId));

        return NoContent();
    }

}
