using Domain.Contracts;
using Domain.Entities.ContentsMudule; 
using Domain.Entities.UserModule;
using Services.Abstraction.Contracts;
using Shared.Dtos;
namespace Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MonthlyReportDto> GetUserMonthlyReportAsync(string userEmail)
        {
            var userRepo = _unitOfWork.GetRepository<User, int>();
            var contentRepo = _unitOfWork.GetRepository<Content, int>();

            var allUsers = await userRepo.GetAllAsync(asNoTracking: true);
            var user = allUsers.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                throw new Exception("User account not found.");
            }

            var startDate = DateTime.UtcNow.AddDays(-30);
            var timeRangeText = $"{startDate:MMM d} - {DateTime.UtcNow:MMM d}";

            var allContents = await contentRepo.GetAllAsync(asNoTracking: true);

            var userContents = allContents
                .Where(c => c.UserEmail == user.Email && c.DetectionDate >= startDate)
                .ToList();

            var textLogs = userContents.OfType<TextContent>().ToList();
            var videoLogs = userContents.OfType<VideoContent>().ToList();

            int textTotal = textLogs.Count;
            int textViolent = textLogs.Count(t => t.ViolentResult == true);
            int textAgainstViolence = textLogs.Count(t =>
                t.ViolentWords != null &&
                t.ViolentWords.Any(word => string.Equals(word, "against", StringComparison.OrdinalIgnoreCase))
            );
            int textNeutral = textTotal - textViolent - textAgainstViolence;

            int videoTotal = videoLogs.Count;
            int videoViolent = videoLogs.Count(v => v.ViolentPercent > 0);
            int videoNonViolent = videoTotal - videoViolent;

            int totalAnalyses = textTotal + videoTotal;
            int totalViolent = textViolent + videoViolent;
            int totalNonViolent = totalAnalyses - totalViolent;

            double violencePercentage = totalAnalyses > 0
                ? Math.Round(((double)totalViolent / totalAnalyses) * 100, 0)
                : 0.0;

            // 5. Structure payload mappings for transmission
            var overallStats = new OverallStatsDto(
                TotalAnalyses: totalAnalyses,
                ViolentIncidents: totalViolent,
                NonViolentAnalyses: totalNonViolent,
                AgainstViolenceAnalyses: textAgainstViolence,
                NeutralTextAnalyses: textNeutral
            );

            var videoStats = new VideoStatsDto(
                TotalAnalyzed: videoTotal,
                ViolentIncidents: videoViolent,
                NonViolentAnalyses: videoNonViolent
            );

            var textStats = new TextStatsDto(
                TotalAnalyzed: textTotal,
                ViolentIncidents: textViolent,
                AgainstViolenceAnalyses: textAgainstViolence,
                NeutralAnalyses: textNeutral
            );

            return new MonthlyReportDto(
                IsMonthlyReportEnabled: user.IsMonthlyReportEnabled,
                OverallViolencePercentage: violencePercentage,
                TimeRange: timeRangeText,
                OverallSummary: overallStats,
                VideoSummary: videoStats,
                TextSummary: textStats
            );
        }

        public async Task UpdateReportPreferenceAsync(string userEmail, UpdateReportSettingsDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<User, int>();

            var allUsers = await userRepo.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                throw new Exception("User account not found.");
            }

            user.IsMonthlyReportEnabled = dto.EnableMonthlyReports;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}