namespace UrbanTrack.Api.Controllers.Responses.Projects
{
    public class CreateProjectRequest
    {        
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string SellerId { get; set; }
        public string Status { get; set; }
    }
}
