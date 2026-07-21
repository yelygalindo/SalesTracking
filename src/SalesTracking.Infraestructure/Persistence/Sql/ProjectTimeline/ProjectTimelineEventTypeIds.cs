namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline
{
    internal static class ProjectTimelineEventTypeIds
    {
        public const int ProjectCreated = 1;
        public const int ProjectUpdated = 2;
        public const int ProjectStatusChanged = 3;
        public const int ProjectProgressUpdated = 4;
        public const int NoteAdded = 6;
        public const int AttachmentUploaded = 7;
        public const int DeliveryCreated = 8;
        public const int DeliveryStatusChanged = 9;
        public const int DeliveryReceiptConfirmed = 10;
    }
}