namespace SalesTracking.Application.UseCases.CustomerReminders.Comands
{
    public class GetCustomerRemindersCommand
    {
        public GetCustomerRemindersCommand(string customerExternalId, bool? completed = null)
        {
            CustomerExternalId = customerExternalId;
            Completed = completed;
        }

        public string CustomerExternalId { get; }
        public bool? Completed { get; }
    }
}
