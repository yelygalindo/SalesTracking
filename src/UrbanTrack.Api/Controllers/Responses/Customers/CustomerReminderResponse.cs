namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class CustomerReminderResponse
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime ReminderAt { get; set; }
        public string AssignedToId { get; set; }
        public bool Completed { get; set; }
    }
}
