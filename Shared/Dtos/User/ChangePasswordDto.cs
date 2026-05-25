namespace Shared.Dtos.User
{
    public record ChangePasswordDto(
        string CurrentPassword, 
        string NewPassword, 
        string ConfirmPassword);
}
