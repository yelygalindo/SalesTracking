using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Auth.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Auth.Rows;
using System.ComponentModel.Design;
using System.Data;
using System.Text.RegularExpressions;

namespace SalesTracking.Infrastructure.Persistence.Sql.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AuthSettings _authSettings;

        public AuthRepository(
            IOptions<DatabaseSettings> databaseOptions,
            IOptions<AuthSettings> authSettings,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _authSettings = authSettings?.Value ?? throw new ArgumentNullException(nameof(authSettings));
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            using var conn = CreateConnection();

            AuthUserRow? row = await conn.QueryFirstOrDefaultAsync<AuthUserRow>(
                AuthRepositoryQueries.GetUserById,
                new { UserId = id });

            if (row == null)
                return null;

            return row.ToAuthUserResponse();
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            using var conn = CreateConnection();
            var affectedRows = await conn.ExecuteAsync(AuthRepositoryQueries.RevokeRefreshToken, new
            {
                TokenHash = refreshToken
            });

            return affectedRows > 0;
        }

        public async Task<AuthTokens?> RefreshTokensAsync(string lastToken)
        {
            if (string.IsNullOrWhiteSpace(lastToken))
                return null;
            using var conn = CreateConnection();

            RefreshTokenUserRow? existingToken = await conn.QueryFirstOrDefaultAsync<RefreshTokenUserRow>(
                AuthRepositoryQueries.SelectRefreshTokenWithUser,
                new { TokenHash = lastToken });

            if (existingToken == null)
                return null;

            if (existingToken.RevokedAtUtc != null ||
                existingToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return null;
            }

            User user = existingToken.ToAuthUserResponse();

            var accessTokenExpiresAtUtc = DateTime.UtcNow.AddHours( _authSettings.AccessTokenExpirationHours);
            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays( _authSettings.RefreshTokenExpirationDays);

            var accessToken = _tokenGenerator.GenerateAccessToken(user, accessTokenExpiresAtUtc);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            await conn.ExecuteAsync(AuthRepositoryQueries.UpdateOldRefreshToken, new
            {
                RefreshTokenId = existingToken.RefreshTokenId,
                NewTokenHash = refreshToken
            });

            await conn.ExecuteAsync(AuthRepositoryQueries.InsertNewRefreshToken, new
            {
                existingToken.UserId,
                TokenHash = refreshToken,
                ExpiresAtUtc = refreshTokenExpiresAtUtc
            });

            return new AuthTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAtUtc = accessTokenExpiresAtUtc
            };
        }

        public async Task<PasswordForgot?> SendForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            using var conn = CreateConnection();
            int? userId = await conn.QueryFirstOrDefaultAsync<int?>(
                AuthRepositoryQueries.GetUserIdByEmail,
                new
                {
                    Email = email
                });

            if (userId == null)
                return null;

            string token = _tokenGenerator.GeneratePasswordResetToken();
            DateTime expiresAtUtc = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenExpirationDays);

            await conn.ExecuteAsync(AuthRepositoryQueries.InsertPasswordResetToken, new
            {
                UserId = userId.Value,
                TokenHash = token,
                ExpiresAtUtc = expiresAtUtc
            });

            return new PasswordForgot
            {
                Email = email,
                Token = token,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            using var conn = CreateConnection();
            PasswordResetTokenRow? resetToken = await conn.QueryFirstOrDefaultAsync<PasswordResetTokenRow>(
                AuthRepositoryQueries.SelectPasswordResetToken,
                new { TokenHash = token });

            if (resetToken == null)
                return false;

            if (resetToken.UsedAtUtc != null ||
                resetToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return false;
            }

            string passwordHash = _passwordHasher.Hash(newPassword);
            var affectedUsers = await conn.ExecuteAsync(AuthRepositoryQueries.UpdateUserPassword, new
            {
                resetToken.UserId,
                PasswordHash = passwordHash
            });

            if (affectedUsers == 0)
                return false;

            await conn.ExecuteAsync(AuthRepositoryQueries.MarkPasswordResetTokenUsed, new
            {
                resetToken.Id
            });

            return true;
        }       

        public async Task<Invitation?> GetInvitationByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using var conn = CreateConnection();

            InvitationRow? invitationRow = await conn.QueryFirstOrDefaultAsync<InvitationRow>(
                AuthRepositoryQueries.GetInvitationByToken,
                new { TokenHash = token });

            if (invitationRow == null)
                return null;

            return new Invitation
            {
                TokenHash = invitationRow.TokenHash,
                Email = invitationRow.Email,
                InvitedBy = invitationRow.InvitedBy,
                CompanyId = invitationRow.CompanyId,
                CompanyName = invitationRow.CompanyName,
                ExpiresAtUtc = invitationRow.ExpiresAtUtc
            };
        }

        public async Task<Invitation?> CreateInvitationAsync(CreateInvitation createInvitation)
        {
            if (createInvitation == null)
                return null;

            using var conn = CreateConnection();
            string token = Guid.NewGuid().ToString("N");
            DateTime expiresAtUtc = DateTime.UtcNow.AddDays(7);

            InvitationRow row = await conn.QuerySingleAsync<InvitationRow>(
                AuthRepositoryQueries.CreateInvitation,
                new
                {
                    TokenHash = token,
                    createInvitation.Email,
                    createInvitation.CompanyId,
                    createInvitation.InvitedBy,
                    ExpiresAtUtc = expiresAtUtc
                });

            return new Invitation
            {
                TokenHash = row.TokenHash,
                Email = row.Email,
                InvitedBy = row.InvitedBy,
                CompanyId = row.CompanyId,
                CompanyName = row.CompanyName,
                ExpiresAtUtc = row.ExpiresAtUtc
            };
        }

        public async Task<AcceptInvitation> AcceptInvitationAsync(AcceptInvitationInput request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token))
                return new AcceptInvitation { Succeeded = false };

            using var conn = CreateConnection();

            var invitation = await conn.QueryFirstOrDefaultAsync(AuthRepositoryQueries.GetInvitation, new
            {
                TokenHash = request.Token
            });

            if (invitation == null)
                return new AcceptInvitation { Succeeded = false };

            if (invitation.AcceptedAtUtc != null || invitation.ExpiresAtUtc <= DateTime.UtcNow)
                return new AcceptInvitation { Succeeded = false };

            var externalId = $"user-{Guid.NewGuid():N}";
            var username = GenerateUsername(request.FullNameUser, invitation.Email);
            var passwordHash = _passwordHasher.Hash(request.Password);

            var newUserId = await conn.QuerySingleAsync<int>(AuthRepositoryQueries.InsertUser, new
            {
                ExternalId = externalId,
                Username = username,
                invitation.Email,
                FullName = request.FullNameUser,
                PasswordHash = passwordHash,
                invitation.CompanyId
            });

            await conn.ExecuteAsync(AuthRepositoryQueries.MarkInvitationAccepted, new
            {
                InvitationId = invitation.Id
            });

            User user = new ()
            {
                Id = newUserId,
                ExternalId = externalId,
                Username = username,
                Email = invitation.Email,
                FullName = request.FullNameUser,
                Company = new Company(invitation.CompanyId,invitation.CompanyExternalId, invitation.CompanyName),              
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };                  


            return new AcceptInvitation
            {
                Succeeded = true,
                User = user
            };
        }

        public async Task<AuthTokens?> ValidateCredentialsAsync(string email, string password)
        {
            using var conn = CreateConnection();
            AuthUserRow? authUserRow = await conn.QueryFirstOrDefaultAsync<AuthUserRow>(AuthRepositoryQueries.SelectUserForEmail, new { Email = email });

            if (authUserRow == null || !_passwordHasher.Verify(password, authUserRow.PasswordHash))
                return null;

            var accessTokenExpiresAtUtc = DateTime.UtcNow.AddHours(_authSettings.AccessTokenExpirationHours);
            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenExpirationDays);
            User user = authUserRow.ToAuthUserResponse();
            var accessToken = _tokenGenerator.GenerateAccessToken(user, accessTokenExpiresAtUtc);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            await conn.ExecuteAsync(AuthRepositoryQueries.InsertRefreshToken, new
            {
                UserId = user.Id,
                TokenHash = refreshToken,
                ExpiresAtUtc = refreshTokenExpiresAtUtc
            });

            return new AuthTokens
            {
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAtUtc = accessTokenExpiresAtUtc
            };
        }

        private static string GenerateUsername(string fullName, string email)
        {
            string baseName;
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                baseName = new string(fullName.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                baseName = email.Split('@')[0].ToLowerInvariant();
            }
            else
            {
                baseName = "user";
            }
            baseName = Regex.Replace(baseName, @"[^a-z0-9]", string.Empty);
            var suffix = Guid.NewGuid().ToString("N").Substring(0, 6);
            return $"{baseName}_{suffix}";
        }
    }
}
