using Microsoft.AspNetCore.Mvc;
using Wajeb.API.Dtos;
using Wajeb.API.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Wajeb.API.Controllers;

[ApiController]
[Route("Auth")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var id))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
                return NotFound();

            var userDetails = await _userService.GetUserDetailsAsync(user);
            return Ok(userDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var id))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
                return NotFound();

            var isPasswordValid = await _userService.VerifyPasswordAsync(user, dto.OldPassword);
            if (!isPasswordValid)
                return BadRequest(new { message = "Old password is incorrect" });

            var hashedPassword = await _userService.HashPasswordAsync(user, dto.NewPassword);
            user.Password = hashedPassword;
            await _userService.UpdateUserAsync(user);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        try
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var id))
                return Unauthorized();

            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting current user");
            return StatusCode(500, "Internal server error");
        }
    }
}