namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments
{
    internal static class ProjectAttachmentQueries
    {
        public const string ProjectExists = @"
SELECT COUNT(1)
FROM Projects
WHERE ExternalId = @ProjectExternalId
  AND CompanyId = @CompanyId
  AND IsDeleted = 0;";

        public const string GetProjectInternalIdByExternalId = @"
SELECT TOP 1 Id
FROM Projects
WHERE ExternalId = @ExternalId
  AND CompanyId = @CompanyId
  AND IsDeleted = 0;";

        public const string GetUserInternalIdByExternalId = @"
SELECT TOP 1 Id
FROM Users
WHERE ExternalId = @ExternalId
  AND CompanyId = @CompanyId
  AND IsActive = 1;";

        public const string Get = @"
SELECT
    pa.Id,
    pa.ExternalId,
    pa.FileName,
    pa.ContentType,
    pa.SizeBytes,
    pa.AttachmentType,
    pa.Caption,
    pa.IsCover,
    u.ExternalId AS UploadedByUserExternalId,
    u.FullName AS UploadedByUserName,
    pa.CreatedAtUtc,
    pa.UpdatedAtUtc
FROM ProjectAttachments pa
INNER JOIN Projects p ON p.Id = pa.ProjectId
INNER JOIN Users u ON u.Id = pa.UploadedByUserId
WHERE p.ExternalId = @ProjectExternalId
  AND p.CompanyId = @CompanyId
  AND pa.CompanyId = @CompanyId
  AND u.CompanyId = @CompanyId
  AND p.IsDeleted = 0
  AND pa.IsDeleted = 0
ORDER BY pa.IsCover DESC, pa.CreatedAtUtc DESC;";

        public const string GetContentInfo = @"
SELECT TOP 1
    pa.FileName,
    pa.ContentType,
    pa.StorageKey
FROM ProjectAttachments pa
INNER JOIN Projects p ON p.Id = pa.ProjectId
WHERE p.ExternalId = @ProjectExternalId
  AND p.CompanyId = @CompanyId
  AND pa.CompanyId = @CompanyId
  AND p.IsDeleted = 0
  AND pa.ExternalId = @AttachmentExternalId
  AND pa.IsDeleted = 0;";

        public const string Insert = @"
INSERT INTO ProjectAttachments (
    ExternalId,
    ProjectId,
    FileName,
    StorageProvider,
    StorageKey,
    ContentType,
    SizeBytes,
    AttachmentType,
    Caption,
    IsCover,
    UploadedByUserId,
    CreatedAtUtc,
    IsDeleted,
    CompanyId
)
OUTPUT INSERTED.Id
VALUES (
    @ExternalId,
    @ProjectId,
    @FileName,
    @StorageProvider,
    @StorageKey,
    @ContentType,
    @SizeBytes,
    @AttachmentType,
    @Caption,
    @IsCover,
    @UploadedByUserId,
    SYSUTCDATETIME(),
    0,
    @CompanyId
);";

        public const string ClearCover = @"
UPDATE ProjectAttachments
SET
    IsCover = 0,
    UpdatedByUserId = @UpdatedByUserId,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE ProjectId = @ProjectId
  AND CompanyId = @CompanyId
  AND IsDeleted = 0
  AND IsCover = 1;";

        public const string Delete = @"
UPDATE pa
SET
    IsDeleted = 1,
    IsCover = 0,
    DeletedByUserId = @DeletedByUserId,
    DeletedAtUtc = SYSUTCDATETIME(),
    UpdatedByUserId = @DeletedByUserId,
    UpdatedAtUtc = SYSUTCDATETIME()
FROM ProjectAttachments pa
INNER JOIN Projects p ON p.Id = pa.ProjectId
WHERE p.ExternalId = @ProjectExternalId
  AND p.CompanyId = @CompanyId
  AND pa.CompanyId = @CompanyId
  AND p.IsDeleted = 0
  AND pa.ExternalId = @AttachmentExternalId
  AND pa.IsDeleted = 0;";

        public const string GetAttachmentInternal = @"
SELECT TOP 1
    pa.Id,
    pa.ProjectId
FROM ProjectAttachments pa
INNER JOIN Projects p ON p.Id = pa.ProjectId
WHERE p.ExternalId = @ProjectExternalId
  AND p.CompanyId = @CompanyId
  AND pa.CompanyId = @CompanyId
  AND p.IsDeleted = 0
  AND pa.ExternalId = @AttachmentExternalId
  AND pa.IsDeleted = 0;";

        public const string SetCover = @"
UPDATE ProjectAttachments
SET
    IsCover = 1,
    UpdatedByUserId = @UpdatedByUserId,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE Id = @AttachmentId
  AND CompanyId = @CompanyId
  AND ProjectId = @ProjectId
  AND IsDeleted = 0;";
    }
}
