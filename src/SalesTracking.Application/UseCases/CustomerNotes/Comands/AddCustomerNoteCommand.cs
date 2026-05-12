namespace SalesTracking.Application.UseCases.CustomerNotes.Comands
{
    public class AddCustomerNoteCommand
    {
        public string CustomerExternalId { get; set; }
        public string Text { get; set; }
        public string AuthorExternalId { get; set; }
    }
}