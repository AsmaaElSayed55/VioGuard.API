using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize] // Protects all endpoints below; requires a valid login token
    [ApiController]
    [Route("/api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _userService.GetUserByEmailAsync(email); if (user is null) return NotFound(new { Message = "User profile not found." });

            return Ok(user);
        }

        [HttpPut("/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            try
            {
                var updatedUser = await _userService.UpdateProfileAsync(email, profileDto);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesDto preferencesDto)
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            try
            {
                var updatedUser = await _userService.UpdatePreferencesAsync(email, preferencesDto);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            if (passwordDto.NewPassword != passwordDto.ConfirmPassword)
            {
                return BadRequest(new { Message = "The new password and confirmation password fields do not match." });
            }

            var success = await _userService.ChangePasswordAsync(email, passwordDto);
            if (!success) return NotFound(new { Message = "User account lookup failed." });

            return Ok(new { Message = "Password has been successfully modified." });
        }
        private string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
