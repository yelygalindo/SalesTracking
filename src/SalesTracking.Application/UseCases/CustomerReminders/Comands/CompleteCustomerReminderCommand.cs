namespace SalesTracking.Application.UseCases.CustomerReminders.Comands
{
    public class CompleteCustomerReminderCommand
    {
        public CompleteCustomerReminderCommand(
            string customerExternalId,
            string reminderExternalId,
            int completedByUserId = 0)
        {
            CustomerExternalId = customerExternalId;
            ReminderExternalId = reminderExternalId;
            CompletedByUserId = completedByUserId;
        }

        public string CustomerExternalId { get; }
        public string ReminderExternalId { get; }
        public int CompletedByUserId { get; }
    }
}
