namespace UrbanTrack.Api.Controllers.Responses.CustomerReminders
{
    public class AssignedReminderResponse
    {
        public AssignedReminderResponse(int id, string externalId, string name)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
        }

        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }        
    }
}
