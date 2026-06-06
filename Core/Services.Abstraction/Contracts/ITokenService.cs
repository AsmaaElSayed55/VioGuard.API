using Shared.Dtos.User;

namespace Services.Abstraction.Contracts
{
    public interface ITokenService
    {
        (string Token, DateTime Expiration) GenerateToken(UserDto user);
    }
}
