using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.Report;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("monthly-dashboard")]
        public async Task<ActionResult<MonthlyReportDashboardDto>> GetDashboardMetrics()
        {
            var metrics = new MonthlyReportDashboardDto(
                2745,                                 // TotalAnalyses
                84,                                   // TotalViolentIncidents
                1203,                                 // TotalNonViolentAnalyses
                458,                                  // TotalAgainstViolenceAnalyses
                1000,                                 // TotalNeutralTextAnalyses
                34.0,                                 // ViolencePercentage
                new VideoSummaryDto(1245, 42, 1203),  // VideoSummaryDto (Total, Violent, NonViolent)
                new TextSummaryDto(1500, 42, 458, 1000), // TextSummaryDto (Total, Violent, AgainstViolence, Neutral)
                true,                                 // EnableMonthlyReports
                new DateTime(2026, 5, 1),             // DateFrom
                new DateTime(2026, 5, 25)             // DateTo
            );

            return Ok(metrics);
        }

        [HttpPost("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateReportSettingsDto dto)
        {
            return Ok(new
            {
                Message = "Report toggle updated successfully.",
                ActiveState = dto.EnableMonthlyReports
            });
        }
    }
}