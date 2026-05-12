namespace SalesTracking.Application.UseCases.CustomerReminders.Comands
{
    public class CompleteCustomerReminderCommand
    {
        public CompleteCustomerReminderCommand(string customerExternalId, string reminderExternalId)
        {
            CustomerExternalId = customerExternalId;
            ReminderExternalId = reminderExternalId;
        }

        public string CustomerExternalId { get; }
        public string ReminderExternalId { get; }
    }
}
