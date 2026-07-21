using SalesTracking.Application.UseCases.ProjectTimeline.Results;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline.Mappers
{
    internal static class ProjectTimelineRepositoryMapper
    {
        public static ProjectTimelineResult ToResult(this ProjectTimelineRow row)
        {
            return new ProjectTimelineResult
            {
                ExternalId = row.ExternalId,
                EventTypeId = row.EventTypeId,
                EventTypeName = row.EventTypeName,
                Title = row.Title,
                Description = row.Description,
                OccurredAtUtc = row.OccurredAtUtc,
                CreatedBy = ToUserResult(row.CreatedByUserExternalId, row.CreatedByUserName),
                RelatedEntityType = row.RelatedEntityType,
                RelatedEntityId = row.RelatedEntityId,
                MetadataJson = row.MetadataJson
            };
        }

        private static ProjectTimelineUserResult? ToUserResult(string? externalId, string? name)
        {
            if (string.IsNullOrWhiteSpace(externalId) && string.IsNullOrWhiteSpace(name))
                return null;

            return new ProjectTimelineUserResult
            {
                ExternalId = externalId ?? string.Empty,
                Name = name
            };
        }
    }
}