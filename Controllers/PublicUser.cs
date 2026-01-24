using Microsoft.AspNetCore.Mvc;
using Wajeb.API.Dtos;
using Wajeb.API.Services;

namespace Wajeb.API.Controllers;


[ApiController]
[Route("users")]
public class PublicUser(
    IUserService userService,
    ILogger<PublicUser> logger) : ControllerBase
{
    private readonly ILogger<PublicUser> _logger = logger;
    private readonly IUserService _userService = userService;

    [HttpGet("{username}")]
    public async Task<ActionResult<GetUserPublic>> GetPublicUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required");
        }

        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user is null)
            {
                return NotFound($"User '{username}' not found");
            }


            var publicUser = new GetUserPublic
            {
                Username = user.Username

            };

            return Ok(publicUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public user");
            return StatusCode(500, "Internal server error");
        }
    }
}