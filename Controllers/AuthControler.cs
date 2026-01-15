using Wajeb.API.Dtos;
using Wajeb.API.Models;
using Wajeb.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Wajeb.API.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUser createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _userService.UserExistsByUsernameAsync(createUserDto.Username))
                    return Conflict(new { message = "Username already exists" });

                if (await _userService.UserExistsByEmailAsync(createUserDto.Email))
                    return Conflict(new { message = "Email already exists" });

                var user = new User
                {
                    Username = createUserDto.Username,
                    Email = createUserDto.Email
                };

                var hashedPassword = await _userService.HashPasswordAsync(user, createUserDto.Password);
                var createdUser = await _userService.CreateUserAsync(createUserDto, hashedPassword);
                var userDetail = await _userService.GetUserDetailsAsync(createdUser);

                return CreatedAtAction(nameof(Register), new { id = createdUser.Id }, userDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.GetUserByEmailAsync(loginDto.Email);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                var isPasswordValid = await _userService.VerifyPasswordAsync(user, loginDto.Password);
                if (!isPasswordValid)
                    return Unauthorized(new { message = "Invalid email or password" });

                var userDetail = await _userService.GetUserDetailsAsync(user);
                string token = GenerateToken(user);

                return Ok(new { user = userDetail, token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                return StatusCode(500, "Internal server error");
            }
        }


        //Toeken Generation method
        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var secret = _configuration["AppSettings:Jwt"] ?? _configuration["AppSettings:Key"];
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT secret not configured. Set AppSettings:Jwt or AppSettings:Key in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // token settings and descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["AppSettings:Issuer"],
                Audience = _configuration["AppSettings:Audience"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}