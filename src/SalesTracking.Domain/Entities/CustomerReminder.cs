namespace SalesTracking.Domain.Entities
{
    public class CustomerReminder
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }        
        public Customer Customer { get; set; }
        public bool Completed { get; set; }
        public DateTime ReminderAtUtc { get; set; }
    }
}
