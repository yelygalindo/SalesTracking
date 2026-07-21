namespace SalesTracking.Application.UseCases.Reports.Results
{
    public sealed class CommercialActivityReportResult
    {
        public string TimelineExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectStatusId { get; set; }
        public string ProjectStatusName { get; set; } = string.Empty;
        public string? SellerExternalId { get; set; }
        public string? SellerName { get; set; }
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string? CreatedByExternalId { get; set; }
        public string? CreatedByName { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? MetadataJson { get; set; }
    }
}
