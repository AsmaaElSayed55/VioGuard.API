using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.Report;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("monthly-dashboard")]
        public async Task<ActionResult<MonthlyReportDashboardDto>> GetDashboardMetrics([FromQuery] string? userEmail = null)
        {
            var email = ResolveUserEmail(userEmail);
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { Message = "User email is required." });

            var metrics = await _reportService.GetMonthlyDashboardMetricsAsync(email);
            return Ok(metrics);
        }

        [HttpPost("settings")]
        public async Task<IActionResult> UpdateSettings(
            [FromBody] UpdateReportSettingsDto dto,
            [FromQuery] string? userEmail = null)
        {
            var email = ResolveUserEmail(userEmail);
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { Message = "User email is required." });

            var updated = await _reportService.UpdateReportSettingsAsync(email, dto);
            if (!updated)
                return NotFound(new { Message = "User account lookup failed." });

            return Ok(new { Message = "Report toggle updated.", ActiveState = dto.EnableMonthlyReports });
        }

        private string? ResolveUserEmail(string? queryEmail)
        {
            if (!string.IsNullOrWhiteSpace(queryEmail))
                return queryEmail;

            return User.FindFirstValue(ClaimTypes.Email);
        }
    }
}
