using System;

namespace Shared.Dtos.User
{
    public record UserDto
    {
        public UserDto() 
        { 

        }
        public string Id { get; init; }
        public string FullName { get; init; }
        public string Email { get; init; }
        public bool IsMonthlyReportEnabled { get; init; }
        public bool IsTwoStepEnabled { get; init; } // For your Identity/Auth layer
        public bool IsDarkMode { get; init; }        // For your UI preferences layer
    }
}