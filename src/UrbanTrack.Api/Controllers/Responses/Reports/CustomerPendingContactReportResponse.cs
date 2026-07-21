namespace UrbanTrack.Api.Controllers.Responses.Reports
{
    public sealed class CustomerPendingContactReportResponse
    {
        public string ReminderExternalId { get; set; } = string.Empty;
        public string CustomerExternalId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int StatusId { get; set; }
        public string? SellerExternalId { get; set; }
        public string? SellerName { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime ReminderAtUtc { get; set; }
        public string AssignedToExternalId { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
    }
}
