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

namespace SalesTracking.Infrastructure.Persistence.Sql.Invitations
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly IPasswordHasher _passwordHasher;

        public InvitationRepository(
            IOptions<DatabaseSettings> databaseOptions,
            IPasswordHasher passwordHasher)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);
        
        public async Task<Invitation?> GetInvitationByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using var conn = CreateConnection();

            InvitationRow? invitationRow = await conn.QueryFirstOrDefaultAsync<InvitationRow>(
                InvitationRepositoryQueries.GetInvitationByToken,
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
                InvitationRepositoryQueries.CreateInvitation,
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

            var invitation = await conn.QueryFirstOrDefaultAsync(InvitationRepositoryQueries.GetInvitation, new
            {
                TokenHash = request.Token
            });

            if (invitation == null)
                return new AcceptInvitation { Succeeded = false };

            if (invitation.AcceptedAtUtc != null || invitation.ExpiresAtUtc <= DateTime.UtcNow)
                return new AcceptInvitation { Succeeded = false };

            string externalId = ExternalIdGenerator.New(ExternalIdPrefixes.User);
            var username = GenerateUsername(request.FullNameUser, invitation.Email);
            var passwordHash = _passwordHasher.Hash(request.Password);

            var newUserId = await conn.QuerySingleAsync<int>(InvitationRepositoryQueries.InsertUser, new
            {
                ExternalId = externalId,
                Username = username,
                invitation.Email,
                FullName = request.FullNameUser,
                PasswordHash = passwordHash,
                invitation.CompanyId
            });

            await conn.ExecuteAsync(InvitationRepositoryQueries.MarkInvitationAccepted, new
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