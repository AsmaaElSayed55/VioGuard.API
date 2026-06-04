using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetProfile(string email)
        {
            var userDto = await _userService.GetUserByEmailAsync(email);
            if (userDto == null) return NotFound(new { Message = "User profile not found." });

            // Returns clean, secure JSON object to the client application without exposing internal entity models
            return Ok(userDto);
        }

        [HttpPut("{email}/profile")]
        public async Task<IActionResult> UpdateProfile(string email, [FromBody] UpdateProfileDto profileDto)
        {
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

        [HttpPut("{email}/preferences")]
        public async Task<IActionResult> UpdatePreferences(string email, [FromBody] UpdatePreferencesDto preferencesDto)
        {
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

        [HttpPost("{email}/change-password")]
        public async Task<IActionResult> ChangePassword(string email, [FromBody] ChangePasswordDto passwordDto)
        {
            if (passwordDto.NewPassword != passwordDto.ConfirmPassword)
            {
                return BadRequest(new { Message = "The new password and confirmation password fields do not match." });
            }

            var success = await _userService.ChangePasswordAsync(email, passwordDto);
            if (!success) return NotFound(new { Message = "User account lookup failed." });

            return Ok(new { Message = "Password has been successfully modified." });
        }
    }
}
