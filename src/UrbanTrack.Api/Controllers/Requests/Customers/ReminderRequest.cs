namespace UrbanTrack.Api.Controllers.Requests.Customers
{
    public sealed record ReminderRequest(
        string Title,
        DateTime DueDate);
}
