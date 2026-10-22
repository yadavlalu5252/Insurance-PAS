
using Authentication.Application.DTO;
using Authentication.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        try
        {
            var result = await userService.CreateUser(request);
            return StatusCode(201, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await userService.GetAllUsers();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetUserById(long id)
    {
        var result = await userService.GetUserById(id);

        if (result == null)
            return NotFound(new { message = "User not found." });

        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto request)
    {
        try
        {
            var result = await userService.UpdateUser(id, request);

            if (result == null)
                return NotFound(new { message = "User not found." });

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
