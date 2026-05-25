using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos;
using System.Security.Claims;
namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Retrieves all data needed for the Monthly Report UI dashboard.
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User context not found.");

            try
            {
                var reportData = await _reportService.GetUserMonthlyReportAsync(userEmail);
                return Ok(reportData);
            }
            catch (System.Exception ex)
            {
                // Returns a clean 404 error with your "User account not found" message
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateReportSettings([FromBody] UpdateReportSettingsDto request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User context not found.");

            await _reportService.UpdateReportPreferenceAsync(userEmail, request);

            // 204 No Content is standard for a successful update that doesn't need to return data
            return NoContent();
        }
    }
}
