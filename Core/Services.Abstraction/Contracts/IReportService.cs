using System.Threading.Tasks;
using Shared.Dtos.Report;

namespace Services.Abstraction.Contracts
{
    public interface IReportService
    {
        Task<MonthlyReportDashboardDto> GetMonthlyDashboardMetricsAsync(string userEmail);
        Task<bool> UpdateReportSettingsAsync(string userEmail, UpdateReportSettingsDto dto);
    }
}