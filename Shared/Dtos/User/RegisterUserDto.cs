using System.ComponentModel.DataAnnotations;
namespace Shared.Dtos.User
{
    public record RegisterUserDto(
        [Required(ErrorMessage = "Full name is required.")]
        string FullName,
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        string Email,
        [Required(ErrorMessage = "Password is required.")]
        string Password,
        [Required(ErrorMessage = "Confirm password is required.")]
        string ConfirmPassword
    );
}
