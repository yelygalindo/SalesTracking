namespace UrbanTrack.Api.Controllers.Requests.CustomerReminders
{
    public class CustomerReminderRequest
    {
        public string Text { get; set; }
        public DateTime ReminderAtUtc { get; set; }
        public string AssignedToId { get; set; }
    }
}