using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var result = await _userService.CreateUserAsync(registerUserDto);

            if (result == null)
            {
                // 💡 RESULT: Postman will get a clean, diagnostic 400 Bad Request response 
                return BadRequest(new { message = "A user with this email address is already registered." });
            }

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password credentials." });
            }

            // In production, add password string verification hash comparison here
            var response = new AuthResponseDto(
                Token: "secure-authenticated-session-token",
                Expiration: DateTime.UtcNow.AddHours(2),
                User: user
            );

            return Ok(response);
        }
    }
}