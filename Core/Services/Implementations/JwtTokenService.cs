using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Dtos.User;

using Services.Abstraction.Contracts;

namespace Services.Implementations
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime Expiration) GenerateToken(UserDto user)
        {
            var key = _configuration["Jwt:Key"] ?? "VioGuardSuperSecretKeyForDevelopmentOnly123!";
            var issuer = _configuration["Jwt:Issuer"] ?? "VioGuard";
            var audience = _configuration["Jwt:Audience"] ?? "VioGuardApp";
            var expiresHours = int.TryParse(_configuration["Jwt:ExpiresHours"], out var hours) ? hours : 24;

            var expiration = DateTime.UtcNow.AddHours(expiresHours);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiration,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}
