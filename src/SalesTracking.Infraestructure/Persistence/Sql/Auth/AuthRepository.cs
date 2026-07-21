using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Security;
using SalesTracking.Infrastructure.Persistence.Sql.Auth.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Auth.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AuthSettings _authSettings;
        private readonly IUserAuthorizationRepository _userAuthorizationRepository;

        public AuthRepository(
            IOptions<DatabaseSettings> databaseOptions,
            IOptions<AuthSettings> authSettings,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator,
            IUserAuthorizationRepository userAuthorizationRepository)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _authSettings = authSettings?.Value ?? throw new ArgumentNullException(nameof(authSettings));
            _userAuthorizationRepository = userAuthorizationRepository;
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
                TokenHash = TokenHasher.Hash(refreshToken.Trim())
            });

            return affectedRows > 0;
        }

        public async Task<AuthTokens?> RefreshTokensAsync(string lastToken)
        {
            if (string.IsNullOrWhiteSpace(lastToken))
                return null;
            using var conn = (SqlConnection)CreateConnection();
            await conn.OpenAsync();
            using SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable);

            RefreshTokenUserRow? existingToken = await conn.QueryFirstOrDefaultAsync<RefreshTokenUserRow>(
                AuthRepositoryQueries.SelectRefreshTokenWithUser,
                new { TokenHash = TokenHasher.Hash(lastToken.Trim()) },
                transaction);

            if (existingToken == null)
            {
                transaction.Rollback();
                return null;
            }

            if (existingToken.RevokedAtUtc != null ||
                existingToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                transaction.Rollback();
                return null;
            }

            User user = existingToken.ToAuthUserResponse();

            var accessTokenExpiresAtUtc = DateTime.UtcNow.AddHours( _authSettings.AccessTokenExpirationHours);
            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays( _authSettings.RefreshTokenExpirationDays);

            var authorization = await _userAuthorizationRepository.GetByUserIdAsync(user.Id);
            var accessToken = _tokenGenerator.GenerateAccessToken(user, authorization, accessTokenExpiresAtUtc);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();
            string refreshTokenHash = TokenHasher.Hash(refreshToken);

            int rotated = await conn.ExecuteAsync(AuthRepositoryQueries.UpdateOldRefreshToken, new
            {
                RefreshTokenId = existingToken.RefreshTokenId,
                NewTokenHash = refreshTokenHash
            }, transaction);

            if (rotated != 1)
            {
                transaction.Rollback();
                return null;
            }

            await conn.ExecuteAsync(AuthRepositoryQueries.InsertNewRefreshToken, new
            {
                existingToken.UserId,
                TokenHash = refreshTokenHash,
                ExpiresAtUtc = refreshTokenExpiresAtUtc
            }, transaction);

            transaction.Commit();

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
            DateTime expiresAtUtc = DateTime.UtcNow.AddHours(_authSettings.PasswordResetTokenExpirationHours);

            await conn.ExecuteAsync(AuthRepositoryQueries.InsertPasswordResetToken, new
            {
                UserId = userId.Value,
                TokenHash = TokenHasher.Hash(token),
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

            using var conn = (SqlConnection)CreateConnection();
            await conn.OpenAsync();
            using SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable);
            PasswordResetTokenRow? resetToken = await conn.QueryFirstOrDefaultAsync<PasswordResetTokenRow>(
                AuthRepositoryQueries.SelectPasswordResetToken,
                new { TokenHash = TokenHasher.Hash(token.Trim()) },
                transaction);

            if (resetToken == null)
            {
                transaction.Rollback();
                return false;
            }

            if (resetToken.UsedAtUtc != null ||
                resetToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                transaction.Rollback();
                return false;
            }

            string passwordHash = _passwordHasher.Hash(newPassword);
            int consumed = await conn.ExecuteAsync(AuthRepositoryQueries.MarkPasswordResetTokenUsed, new
            {
                resetToken.Id
            }, transaction);

            if (consumed != 1)
            {
                transaction.Rollback();
                return false;
            }

            var affectedUsers = await conn.ExecuteAsync(AuthRepositoryQueries.UpdateUserPassword, new
            {
                resetToken.UserId,
                PasswordHash = passwordHash
            }, transaction);

            if (affectedUsers == 0)
            {
                transaction.Rollback();
                return false;
            }

            await conn.ExecuteAsync(
                AuthRepositoryQueries.RevokeUserRefreshTokens,
                new { resetToken.UserId },
                transaction);

            transaction.Commit();

            return true;
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
            var authorization = await _userAuthorizationRepository.GetByUserIdAsync(user.Id);
            var accessToken = _tokenGenerator.GenerateAccessToken(user, authorization, accessTokenExpiresAtUtc);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            await conn.ExecuteAsync(AuthRepositoryQueries.InsertRefreshToken, new
            {
                UserId = user.Id,
                TokenHash = TokenHasher.Hash(refreshToken),
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
    }
}
