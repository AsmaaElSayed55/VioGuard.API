namespace Shared.Dtos
{
    public record MonthlyReportDto(
         bool IsMonthlyReportEnabled,
         double OverallViolencePercentage,
         string TimeRange,                
         OverallStatsDto OverallSummary,
         VideoStatsDto VideoSummary,
         TextStatsDto TextSummary
     );
}
