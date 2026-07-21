namespace SalesTracking.Application.UseCases.Dashboard.Results
{
    public sealed class DashboardUrgentDeliveryResult
    {
        public string DeliveryExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public bool IsOverdue { get; set; }
    }
}
