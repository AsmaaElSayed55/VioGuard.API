namespace Shared.Dtos.User
{
    public record UpdatePreferencesDto(
        bool IsDarkMode,
        bool IsMonthlyReportEnabled
    );
}
