namespace SalesTracking.Application.UseCases.CustomerReminders.Comands
{
    public class GetCustomerRemindersCommand
    {
        public GetCustomerRemindersCommand(string customerExternalId)
        {
            CustomerExternalId = customerExternalId;
        }

        public string CustomerExternalId { get; }
    }
}