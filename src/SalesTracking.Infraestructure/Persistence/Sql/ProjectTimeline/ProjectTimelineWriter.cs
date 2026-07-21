using Dapper;
using SalesTracking.Application.Common.ExternalIds;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline
{
    internal static class ProjectTimelineWriter
    {
        public static async Task InsertAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            ProjectTimelineEvent timelineEvent)
        {
            await connection.ExecuteAsync(
                ProjectTimelineQueries.Insert,
                new
                {
                    ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.ProjectTimelineEvent),
                    timelineEvent.ProjectId,
                    timelineEvent.EventTypeId,
                    timelineEvent.Title,
                    timelineEvent.Description,
                    timelineEvent.OccurredAtUtc,
                    timelineEvent.CreatedByUserId,
                    timelineEvent.RelatedEntityType,
                    timelineEvent.RelatedEntityId,
                    timelineEvent.MetadataJson
                },
                transaction);
        }
    }
}