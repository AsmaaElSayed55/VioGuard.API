using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction.Contracts;
using Shared.Dtos.Report;

namespace Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IApplicationDbContext _context;

        public ReportService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MonthlyReportDashboardDto> GetMonthlyDashboardMetricsAsync(string userEmail)
        {
            var dateTo = DateTime.UtcNow;
            var dateFrom = dateTo.AddDays(-30);

            // حساب تجميعات الفيديوهات مباشرة من الداتابيز (Server-Side)
            var videoStats = await _context.VideoContents
                .Where(v => v.UserEmail == userEmail && v.DetectionDate >= dateFrom && v.DetectionDate <= dateTo)
                .GroupBy(v => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Violent = g.Count(v => v.ViolentPercent > 40.0)
                })
                .FirstOrDefaultAsync();

            int videoTotal = videoStats?.Total ?? 0;
            int videoViolent = videoStats?.Violent ?? 0;
            int videoNonViolent = videoTotal - videoViolent;

            // حساب تجميعات النصوص مباشرة من الداتابيز (Server-Side)
            var textStats = await _context.TextContents
                .Where(t => t.UserEmail == userEmail && t.DetectionDate >= dateFrom && t.DetectionDate <= dateTo)
                .GroupBy(t => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Violent = g.Count(t => EF.Functions.Like(t.ViolentResult, "%Violent%")),
                    Against = g.Count(t => EF.Functions.Like(t.ViolentResult, "%Against%"))
                })
                .FirstOrDefaultAsync();

            int textTotal = textStats?.Total ?? 0;
            int textViolent = textStats?.Violent ?? 0;
            int textAgainstViolence = textStats?.Against ?? 0;
            int textNeutral = textTotal - (textViolent + textAgainstViolence);

            var totalAnalyses = videoTotal + textTotal;
            var totalViolent = videoViolent + textViolent;

            double violencePercentage = 0;
            if (totalAnalyses > 0)
            {
                violencePercentage = Math.Round(((double)totalViolent / totalAnalyses) * 100, 1);
            }

            return new MonthlyReportDashboardDto(
                TotalAnalyses: totalAnalyses,
                TotalViolentIncidents: totalViolent,
                TotalNonViolentAnalyses: videoNonViolent,
                TotalAgainstViolenceAnalyses: textAgainstViolence,
                TotalNeutralTextAnalyses: textNeutral,
                ViolencePercentage: violencePercentage,
                VideoSummary: new VideoSummaryDto(videoTotal, videoViolent, videoNonViolent),
                TextSummary: new TextSummaryDto(textTotal, textViolent, textAgainstViolence, textNeutral),
                EnableMonthlyReports: true,
                DateFrom: dateFrom,
                DateTo: dateTo
            );
        }

        public async Task<bool> UpdateReportSettingsAsync(string userEmail, UpdateReportSettingsDto dto)
        {
            await Task.Delay(10);
            return true;
        }
    }
}