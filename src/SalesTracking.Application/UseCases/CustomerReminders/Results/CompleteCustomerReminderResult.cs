namespace SalesTracking.Application.UseCases.CustomerReminders.Results
{
    public class CompleteCustomerReminderResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
    }
}
