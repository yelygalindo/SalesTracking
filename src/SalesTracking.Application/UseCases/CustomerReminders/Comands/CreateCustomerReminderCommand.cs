namespace SalesTracking.Application.UseCases.CustomerReminders.Comands
{
    public class CreateCustomerReminderCommand
    {
        public string CustomerExternalId { get; set; }
        public string Text { get; set; }
        public DateTime ReminderAtUtc { get; set; }
        public string AssignedToId { get; set; }
        public int CreatedByUserId { get; set; }
    }
}