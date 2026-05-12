namespace SalesTracking.Application.UseCases.CustomerNotes.Models
{
    public class CreateCustomerNote
    {
        public string ExternalId { get; set; }
        public string CustomerExternalId { get; set; }
        public string Text { get; set; }
        public string AuthorExternalId { get; set; }
    }
}