namespace SalesTracking.Application.UseCases.CustomerReminders.Models
{
    public class CreateCustomerReminder
    {
        public string ExternalId { get; set; }

        public string CustomerExternalId { get; set; }
        public string Text { get; set; }
        public DateTime ReminderAtUtc { get; set; }
        public string AssignedToId { get; set; }
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Message { get; set; }
    }
}
