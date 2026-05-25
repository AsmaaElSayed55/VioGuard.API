using System;

namespace Shared.Dtos.User
{
    public record UserDto(
        string Id,
        string FullName,
        string Email,
        bool IsMonthlyReportEnabled,
        bool IsTwoStepEnabled, // For your Identity/Auth layer
        bool IsDarkMode        // For your UI preferences layer
    );
}