namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Rows
{
    public class CustomerReminderRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public DateTime ReminderAtUtc { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedToExternalId { get; set; }
        public string AssignedToName { get; set; }
        public bool Completed { get; set; }
    }
}
