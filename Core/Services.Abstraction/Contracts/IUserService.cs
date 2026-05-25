using Shared.Dtos.User;

namespace Services.Abstraction.Contracts
{
    public interface IUserService
    {
        // Retrieval
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        // Authentication / Onboarding
        Task<UserDto> CreateUserAsync(RegisterUserDto registerUserDto);
        Task<bool> ChangePasswordAsync(string email, ChangePasswordDto changePasswordDto);

        // Management
        Task<UserDto> UpdateProfileAsync(string email, UpdateProfileDto updateProfileDto);
        Task<UserDto> UpdatePreferencesAsync(string email, UpdatePreferencesDto updatePreferencesDto);
    }
}