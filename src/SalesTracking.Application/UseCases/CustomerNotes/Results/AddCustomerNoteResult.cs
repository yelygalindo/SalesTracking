namespace SalesTracking.Application.UseCases.CustomerNotes.Results
{
    public class AddCustomerNoteResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; }
    }
}
