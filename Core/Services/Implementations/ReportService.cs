using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities.ContentsMudule; // Matches your solution spelling namespace
using Domain.Entities.UserModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.Report;

namespace Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MonthlyReportDashboardDto> GetMonthlyDashboardMetricsAsync(string userEmail)
        {
            var contentRepo = _unitOfWork.GetRepository<Content, string>();
            var userRepo = _unitOfWork.GetRepository<User, string>();

            // Get user settings to see if reports are enabled
            var user = await userRepo.GetByIdAsync(userEmail);
            bool isReportEnabled = user?.IsMonthlyReportEnabled ?? false;

            // Gather all content logs for this user in the last 30 days
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var contents = await contentRepo.GetAllAsync(asNoTracking: true);

            var userLogs = contents
                .Where(c => c.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase) && c.DetectionDate >= thirtyDaysAgo)
                .ToList();

            // Split metrics (Table-Per-Hierarchy model evaluation)
            var videoLogs = userLogs.OfType<VideoContent>().ToList();
            var textLogs = userLogs.OfType<TextContent>().ToList();

            // Calculate text segments
            int textViolent = textLogs.Count(t => t.ViolentResult.Contains("Violent", StringComparison.OrdinalIgnoreCase));
            int textAgainst = textLogs.Count(t => t.ViolentResult.Contains("Safe", StringComparison.OrdinalIgnoreCase));
            int textNeutral = textLogs.Count - (textViolent + textAgainst);

            // Calculate video segments
            int videoViolent = videoLogs.Count(v => v.ViolentPercent > 25.0); // example threshold rule
            int videoNonViolent = videoLogs.Count - videoViolent;

            int totalAnalyses = userLogs.Count;
            int totalViolent = textViolent + videoViolent;

            double violencePercent = totalAnalyses > 0
                ? Math.Round(((double)totalViolent / totalAnalyses) * 100, 1)
                : 0.0;

            return new MonthlyReportDashboardDto(
                TotalAnalyses: totalAnalyses,
                TotalViolentIncidents: totalViolent,
                TotalNonViolentAnalyses: videoNonViolent,
                TotalAgainstViolenceAnalyses: textAgainst,
                TotalNeutralTextAnalyses: textNeutral,
                ViolencePercentage: violencePercent,
                VideoSummary: new VideoSummaryDto(videoLogs.Count, videoViolent, videoNonViolent),
                TextSummary: new TextSummaryDto(textLogs.Count, textViolent, textAgainst, textNeutral),
                EnableMonthlyReports: isReportEnabled,
                DateFrom: thirtyDaysAgo,
                DateTo: DateTime.UtcNow
            );
        }

        public async Task<bool> UpdateReportSettingsAsync(string userEmail, UpdateReportSettingsDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<User, string>();
            var user = await userRepo.GetByIdAsync(userEmail);

            if (user == null) return false;

            user.IsMonthlyReportEnabled = dto.EnableMonthlyReports;
            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}