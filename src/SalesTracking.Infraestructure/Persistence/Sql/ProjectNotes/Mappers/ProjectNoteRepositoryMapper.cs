using SalesTracking.Application.UseCases.ProjectNotes.Results;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Mappers
{
    internal static class ProjectNoteRepositoryMapper
    {
        public static ProjectNoteResult ToResult(this ProjectNoteRow row)
        {
            return new ProjectNoteResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Content = row.Content,
                CreatedBy = ToUserResult(
                    row.CreatedByUserId,
                    row.CreatedByUserExternalId,
                    row.CreatedByUserName),
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedBy = ToUserResult(
                    row.UpdatedByUserId,
                    row.UpdatedByUserExternalId,
                    row.UpdatedByUserName),
                UpdatedAtUtc = row.UpdatedAtUtc
            };
        }

        private static ProjectNoteUserResult? ToUserResult(int? id, string? externalId, string? name)
        {
            if (id == null && string.IsNullOrWhiteSpace(externalId) && string.IsNullOrWhiteSpace(name))
                return null;

            return new ProjectNoteUserResult
            {
                Id = id,
                ExternalId = externalId,
                Name = name
            };
        }
    }
}
