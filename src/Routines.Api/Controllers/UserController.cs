using System.ComponentModel;
using Routines.Api.Attributes;
using Routines.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Routines.Api.Contracts.Requests.Users;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Description("Create a new User.")]
    [HttpPost("users")]
    public async Task<IActionResult> Create([FromBody] UserRequest request)
    {
        var user = request.ToUser();

        await _userService.CreateAsync(user);

        var userResponse = user.ToUserResponse();

        return CreatedAtAction("Get", new { userResponse.Id }, userResponse);
    }

    [Description("Get a User by ID.")]
    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var user = await _userService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        var userResponse = user.ToUserResponse();
        return Ok(userResponse);
    }
    
    [Description("Get all Users.")]
    [HttpGet("users")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        var usersResponse = users.ToUsersResponse();
        return Ok(usersResponse);
    }
    
    [Description("Update a new User by ID.")]
    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateUserRequest request)
    {
        var user = await _userService.GetAsync(request.Id);

        if (user is null)
        {
            return NotFound();
        }

        user.FullName = request.User.FullName ?? user.FullName;
        user.Email = request.User.Email ?? user.Email;
        await _userService.UpdateAsync(user);

        var userResponse = user.ToUserResponse();
        return Ok(userResponse);
    }
    
    [Description("Delete a User by ID.")]
    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
