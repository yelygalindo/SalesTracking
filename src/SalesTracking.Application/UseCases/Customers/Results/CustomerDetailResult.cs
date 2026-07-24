using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class CustomerDetailResult
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public CustomerStatus Status { get; set; }             
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public SellerResult SellerResult { get; set; }
        public List<CustomerNoteResult> Notes { get; set; } = new();
        public List<SalesTracking.Application.UseCases.CustomerReminders.Results.CustomerReminderResult> Reminders { get; set; } = new();
    }
}
