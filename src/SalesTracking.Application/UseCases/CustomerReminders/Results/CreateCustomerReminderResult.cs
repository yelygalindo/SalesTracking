namespace SalesTracking.Application.UseCases.CustomerReminders.Results
{
    public class CreateCustomerReminderResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; }
    }
}
