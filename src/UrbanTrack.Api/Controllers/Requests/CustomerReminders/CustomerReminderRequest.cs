namespace UrbanTrack.Api.Controllers.Requests.CustomerReminders
{
    public class CustomerReminderRequest
    {
        public string Text { get; set; }
        public DateTime ReminderAt { get; set; }
        public string AssignedToId { get; set; }
    }
}