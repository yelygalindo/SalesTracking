namespace UrbanTrack.Api.Controllers.Responses.CustomerReminders
{
    public class CustomerReminderResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public DateTime ReminderAt { get; set; }
        public AssignedReminderResponse AssignedTo { get; set; }
        public bool Completed { get; set; }
    }
}