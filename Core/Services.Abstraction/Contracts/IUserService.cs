using Shared.Dtos.User;

namespace Services.Abstraction.Contracts
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> CreateUserAsync(RegisterUserDto registerUserDto);
        Task<UserDto?> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(string email, ChangePasswordDto changePasswordDto);
        Task<UserDto> UpdateProfileAsync(string email, UpdateProfileDto updateProfileDto);
        Task<UserDto> UpdatePreferencesAsync(string email, UpdatePreferencesDto updatePreferencesDto);
    }
}
