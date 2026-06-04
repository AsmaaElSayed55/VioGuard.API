namespace Shared.Dtos.Report
{
    public record MonthlyReportDashboardDto(
        int TotalAnalyses,
        int TotalViolentIncidents,
        int TotalNonViolentAnalyses,
        int TotalAgainstViolenceAnalyses,
        int TotalNeutralTextAnalyses,
        double ViolencePercentage,
        VideoSummaryDto VideoSummary,
        TextSummaryDto TextSummary,
        bool EnableMonthlyReports,
        DateTime DateFrom,
        DateTime DateTo
    );
}
