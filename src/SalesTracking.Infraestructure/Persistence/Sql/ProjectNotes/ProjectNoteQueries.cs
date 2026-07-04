namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes
{
    internal static class ProjectNoteQueries
    {
        public const string GetByProjectExternalId = @"
SELECT
    pn.Id,
    pn.ExternalId,
    pn.Content,
    cu.Id AS CreatedByUserId,
    cu.ExternalId AS CreatedByUserExternalId,
    cu.FullName AS CreatedByUserName,
    pn.CreatedAtUtc,
    uu.Id AS UpdatedByUserId,
    uu.ExternalId AS UpdatedByUserExternalId,
    uu.FullName AS UpdatedByUserName,
    pn.UpdatedAtUtc
FROM ProjectNotes pn
INNER JOIN Projects p ON p.Id = pn.ProjectId
LEFT JOIN Users cu ON cu.Id = pn.CreatedByUserId
LEFT JOIN Users uu ON uu.Id = pn.UpdatedByUserId
WHERE p.ExternalId = @ProjectExternalId
  AND p.IsDeleted = 0
  AND pn.IsDeleted = 0
ORDER BY pn.CreatedAtUtc DESC;";
    }
}
