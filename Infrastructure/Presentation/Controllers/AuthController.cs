using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        // Constructor kept intact to protect against Dependency Injection resolution failures
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            // 🚀 FIXED: Removed the name labels to map positionally by order
            var staticRegisteredUser = new UserDto(
                "usr_mock_reg_7712",
                registerUserDto.FullName ?? "Static Registered User",
                registerUserDto.Email ?? "registered@vioguard.com",
                true,
                false,
                true
            );

            return Ok(staticRegisteredUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var staticLoggedInUser = new UserDto(
                "usr_mock_login_9943",
                "Mock Operational User",
                loginDto.Email ?? "activeuser@vioguard.com",
                true,
                false,
                false
            );

            // Also removing labels here just in case AuthResponseDto uses camelCase too!
            var response = new AuthResponseDto(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.staticMockTokenBlobValueHere1234567890",
                DateTime.UtcNow.AddHours(2),
                staticLoggedInUser
            );

            return Ok(response);
        }
    }
}