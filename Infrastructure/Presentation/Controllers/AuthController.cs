using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (string.IsNullOrWhiteSpace(registerUserDto.Email) || string.IsNullOrWhiteSpace(registerUserDto.Password))
                return BadRequest(new { Message = "Email and password are required." });

            var user = await _userService.CreateUserAsync(registerUserDto);
            if (user is null)
                return Conflict(new { Message = "A user with this email already exists." });

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest(new { Message = "Email and password are required." });

            var user = await _userService.AuthenticateAsync(loginDto);
            if (user is null)
                return Unauthorized(new { Message = "Invalid email or password." });

            var (token, expiration) = _tokenService.GenerateToken(user);
            return Ok(new AuthResponseDto(token, expiration, user));
        }
    }
}
