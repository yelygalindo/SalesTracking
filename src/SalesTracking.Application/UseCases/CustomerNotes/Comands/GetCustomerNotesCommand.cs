namespace SalesTracking.Application.UseCases.CustomerNotes.Comands
{
    public class GetCustomerNotesCommand
    {
        public GetCustomerNotesCommand(string customerExternalId)
        {
            CustomerExternalId = customerExternalId;
        }

        public string CustomerExternalId { get; }
    }
}