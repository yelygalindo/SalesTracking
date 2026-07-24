using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Auth;
using SalesTracking.Infrastructure.Persistence.Sql.Invitations.Rows;
using System.Data;
using System.Text.RegularExpressions;
using SalesTracking.Infrastructure.Persistence.Security;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace SalesTracking.Infrastructure.Persistence.Sql.Invitations
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUser _currentUser;

        public InvitationRepository(
            IOptions<DatabaseSettings> databaseOptions,
            IPasswordHasher passwordHasher,
            ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _currentUser = currentUser;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);
        
        public async Task<Invitation?> GetInvitationByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using var conn = CreateConnection();

            InvitationRow? invitationRow = await conn.QueryFirstOrDefaultAsync<InvitationRow>(
                InvitationRepositoryQueries.GetInvitationByToken,
                new { TokenHash = TokenHasher.Hash(token.Trim()) });

            if (invitationRow == null)
                return null;

            return new Invitation
            {
                TokenHash = invitationRow.TokenHash,
                Email = invitationRow.Email,
                FullName = invitationRow.FullName,
                RoleCode = invitationRow.RoleCode,
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
            conn.Open();
            using IDbTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable);
            string token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
            string tokenHash = TokenHasher.Hash(token);
            DateTime expiresAtUtc = DateTime.UtcNow.AddDays(7);
            try
            {
                int? roleId = await conn.QueryFirstOrDefaultAsync<int?>(
                    InvitationRepositoryQueries.GetAssignableRoleId,
                    new { createInvitation.RoleCode, createInvitation.InviterPermissions }, transaction);
                if (roleId == null)
                {
                    transaction.Rollback();
                    return null;
                }

                var parameters = new { CompanyId = _currentUser.CompanyId, createInvitation.Email };
                if (await conn.ExecuteScalarAsync<int>(InvitationRepositoryQueries.EmailExistsInCompany, parameters, transaction) > 0 ||
                    await conn.ExecuteScalarAsync<int>(InvitationRepositoryQueries.PendingInvitationExists, parameters, transaction) > 0)
                {
                    transaction.Rollback();
                    return null;
                }

                await conn.ExecuteAsync(InvitationRepositoryQueries.CreateInvitation, new
                {
                    TokenHash = tokenHash,
                    createInvitation.Email,
                    createInvitation.FullName,
                    CompanyId = _currentUser.CompanyId,
                    InvitedBy = _currentUser.UserExternalId,
                    RoleId = roleId.Value,
                    ExpiresAtUtc = expiresAtUtc
                }, transaction);
                transaction.Commit();

                return new Invitation
                {
                    Email = createInvitation.Email,
                    FullName = createInvitation.FullName,
                    RoleCode = createInvitation.RoleCode,
                    InvitedBy = _currentUser.UserExternalId,
                    CompanyId = _currentUser.CompanyId,
                    ExpiresAtUtc = expiresAtUtc
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return null;
            }
        }

        public async Task<AcceptInvitation> AcceptInvitationAsync(AcceptInvitationInput request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token))
                return new AcceptInvitation { Succeeded = false };

            using var conn = CreateConnection();
            conn.Open();
            using IDbTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable);

            var invitation = await conn.QueryFirstOrDefaultAsync(
                InvitationRepositoryQueries.GetInvitationForAcceptance,
                new { TokenHash = TokenHasher.Hash(request.Token.Trim()) }, transaction);

            if (invitation == null)
            {
                transaction.Rollback();
                return new AcceptInvitation { Succeeded = false };
            }

            if (invitation.AcceptedAtUtc != null || invitation.ExpiresAtUtc <= DateTime.UtcNow)
            {
                transaction.Rollback();
                return new AcceptInvitation { Succeeded = false };
            }

            if (await conn.ExecuteScalarAsync<int>(InvitationRepositoryQueries.EmailExistsInCompany,
                    new { invitation.CompanyId, invitation.Email }, transaction) > 0)
            {
                transaction.Rollback();
                return new AcceptInvitation { Succeeded = false };
            }

            string externalId = ExternalIdGenerator.New(ExternalIdPrefixes.User);
            var username = GenerateUsername((string)invitation.FullName, (string)invitation.Email);
            var passwordHash = _passwordHasher.Hash(request.Password);

            var newUserId = await conn.QuerySingleAsync<int>(InvitationRepositoryQueries.InsertUser, new
            {
                ExternalId = externalId,
                Username = username,
                invitation.Email,
                FullName = invitation.FullName,
                PasswordHash = passwordHash,
                invitation.CompanyId
            }, transaction);

            await conn.ExecuteAsync(InvitationRepositoryQueries.InsertUserRole,
                new { UserId = newUserId, invitation.RoleId }, transaction);

            int accepted = await conn.ExecuteAsync(InvitationRepositoryQueries.MarkInvitationAccepted, new
            {
                InvitationId = invitation.Id
            }, transaction);
            if (accepted != 1)
            {
                transaction.Rollback();
                return new AcceptInvitation { Succeeded = false };
            }

            transaction.Commit();

            User user = new ()
            {
                Id = newUserId,
                ExternalId = externalId,
                Username = username,
                Email = invitation.Email,
                FullName = invitation.FullName,
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
