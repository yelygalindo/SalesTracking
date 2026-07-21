namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline
{
    internal static class ProjectTimelineEventTypeIds
    {
        public const int ProjectCreated = 1;
        public const int ProjectUpdated = 2;
        public const int ProjectStatusChanged = 3;
        public const int ProjectProgressUpdated = 4;
        public const int ProjectVisited = 5;
        public const int NoteAdded = 6;
        public const int AttachmentUploaded = 7;
        public const int DeliveryCreated = 8;
        public const int DeliveryStatusChanged = 9;
        public const int DeliveryReceiptConfirmed = 10;
        public const int ProjectDeleted = 11;
        public const int NoteUpdated = 12;
        public const int NoteDeleted = 13;
        public const int AttachmentDeleted = 14;
        public const int AttachmentCoverChanged = 15;
        public const int DeliveryUpdated = 16;
        public const int DeliveryDeleted = 17;
        public const int DeliveryCompleted = 18;
    }
}
