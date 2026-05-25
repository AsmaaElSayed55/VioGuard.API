using System;

namespace Shared.Dtos.User
{
    public record UserDto
    {
        public UserDto(string id, string fullName, string email, bool isMonthlyReportEnabled, bool isTwoStepEnabled, bool isDarkMode)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            IsMonthlyReportEnabled = isMonthlyReportEnabled;
            IsTwoStepEnabled = isTwoStepEnabled;
            IsDarkMode = isDarkMode;
        }
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsMonthlyReportEnabled { get; set; }
        public bool IsTwoStepEnabled { get; set; } // For your Identity/Auth layer
        public bool IsDarkMode { get; set; }        // For your UI preferences layer
    }
}