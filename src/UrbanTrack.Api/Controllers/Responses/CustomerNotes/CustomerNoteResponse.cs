using UrbanTrack.Api.Controllers.Responses.AuthResponses;
using UrbanTrack.Api.Controllers.Responses.Common;

namespace UrbanTrack.Api.Controllers.Responses.CustomerNotes
{
    public class CustomerNoteResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public UserResponse Author { get; set; }        
        public DateTime CreatedAt { get; set; }
    }
}