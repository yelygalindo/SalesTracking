using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SalesTracking.Infrastructure.Persistence.Security
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateAccessToken(User user, DateTime expiresAtUtc)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("userExternalId", user.ExternalId ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? user.Username ?? user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("companyId", user.Company.Id.ToString())
            };

            var keyBytes = Encoding.UTF8.GetBytes(_settings.Secret);

            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT Secret must be at least 32 bytes long.");
            
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordResetToken()
        {
            return GenerateSecureToken();
        }

        public string GenerateRefreshToken()
        {
            return GenerateSecureToken();
        }

        private static string GenerateSecureToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Base64UrlEncoder.Encode(bytes);
        }
    }
}
