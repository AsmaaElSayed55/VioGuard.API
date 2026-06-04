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

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
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

            var response = new AuthResponseDto(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.staticMockTokenBlobValueHere1234567890",
                DateTime.UtcNow.AddHours(2),
                staticLoggedInUser
            );

            return Ok(response);
        }
    }
}