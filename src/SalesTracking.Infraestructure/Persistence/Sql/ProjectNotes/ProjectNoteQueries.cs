namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes
{
    internal static class ProjectNoteQueries
    {
        public const string GetProjectInternalIdByExternalId = @"
SELECT TOP 1
    Id
FROM Projects
WHERE ExternalId = @ExternalId
  AND IsDeleted = 0;";

        public const string GetUserInternalIdByExternalId = @"
SELECT TOP 1
    Id
FROM Users
WHERE ExternalId = @ExternalId
  AND IsActive = 1;";

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

        public const string GetByExternalId = @"
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
  AND pn.ExternalId = @NoteExternalId
  AND pn.IsDeleted = 0;";

        public const string AddNote = @"
INSERT INTO ProjectNotes (
    ExternalId,
    ProjectId,
    Content,
    CreatedByUserId,
    CreatedAtUtc,
    IsDeleted
)
OUTPUT INSERTED.Id
VALUES (
    @ExternalId,
    @ProjectId,
    @Content,
    @AuthorId,
    SYSUTCDATETIME(),
    0
);";

        public const string UpdateNote = @"
UPDATE pn
SET
    pn.Content = @Content,
    pn.UpdatedByUserId = @UpdatedByUserId,
    pn.UpdatedAtUtc = SYSUTCDATETIME()
FROM ProjectNotes pn
INNER JOIN Projects p ON p.Id = pn.ProjectId
WHERE p.ExternalId = @ProjectExternalId
  AND p.IsDeleted = 0
  AND pn.ExternalId = @NoteExternalId
  AND pn.IsDeleted = 0;";

        public const string DeleteNote = @"
UPDATE pn
SET
    pn.IsDeleted = 1,
    pn.UpdatedAtUtc = SYSUTCDATETIME()
FROM ProjectNotes pn
INNER JOIN Projects p ON p.Id = pn.ProjectId
WHERE p.ExternalId = @ProjectExternalId
  AND p.IsDeleted = 0
  AND pn.ExternalId = @NoteExternalId
  AND pn.IsDeleted = 0;";
    }
}