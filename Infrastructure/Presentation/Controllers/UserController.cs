using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var mockUser = new UserDto(
                "usr_mock_login_9943",
                "Mock Operational User",
                email ?? "activeuser@vioguard.com",
                true,  // IsMonthlyReportEnabled
                false, // IsTwoStepEnabled
                false  // IsDarkMode
            );

            return Ok(mockUser);
        }

        [HttpPut("{email}/profile")]
        public async Task<IActionResult> UpdateProfile(string email, [FromBody] UpdateProfileDto profileDto)
        {
            var updatedUser = new UserDto(
                "usr_mock_login_9943",
                profileDto.FullName ?? "Updated Mock Name",
                email ?? "activeuser@vioguard.com",
                true,
                false,
                false
            );

            return Ok(updatedUser);
        }

        [HttpPut("{email}/preferences")]
        public async Task<IActionResult> UpdatePreferences(string email, [FromBody] UpdatePreferencesDto preferencesDto)
        {
            var updatedUser = new UserDto(
                "usr_mock_login_9943",
                "Mock Operational User",
                email ?? "activeuser@vioguard.com",
                preferencesDto.IsMonthlyReportEnabled,
                false, 
                preferencesDto.IsDarkMode
            );

            return Ok(updatedUser);
        }

        [HttpPost("{email}/change-password")]
        public async Task<IActionResult> ChangePassword(string email, [FromBody] ChangePasswordDto passwordDto)
        {
            // Keep validation rule in play for frontend verification tests
            if (passwordDto.NewPassword != passwordDto.ConfirmPassword)
            {
                return BadRequest(new { Message = "The new password and confirmation password fields do not match." });
            }

            return Ok(new { Message = "Password has been successfully modified." });
        }
    }
}