using Shared.Dtos;
namespace Services.Abstraction.Contracts
{
    public interface IReportService
    {
        // the dashboard data for a specific user
        Task<MonthlyReportDto> GetUserMonthlyReportAsync(string userEmail);

        // Updates the database when the user toggles the setting
        Task UpdateReportPreferenceAsync(string userEmail, UpdateReportSettingsDto dto);
    }
}
