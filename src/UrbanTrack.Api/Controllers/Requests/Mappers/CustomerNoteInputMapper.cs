using SalesTracking.Application.UseCases.CustomerNotes.Comands;
using UrbanTrack.Api.Controllers.Requests.CustomerNotes;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class CustomerNoteInputMapper
    {
        public static AddCustomerNoteCommand ToApplication(
            this CustomerNoteRequest request,
            string customerId,
            int authorUserId)
        {
            return new AddCustomerNoteCommand
            {
                CustomerExternalId = customerId,
                Text = request.Text,
                AuthorUserId = authorUserId
            };
        }
    }
}