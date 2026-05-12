using SalesTracking.Application.UseCases.CustomerNotes.Comands;
using UrbanTrack.Api.Controllers.Requests.CustomerNotes;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class CustomerNoteInputMapper
    {
        public static AddCustomerNoteCommand ToApplication(
        this CustomerNoteRequest request,
        string customerId)
        {
            return new AddCustomerNoteCommand
            {
                CustomerExternalId = customerId,
                Text = request.Text,
                AuthorExternalId = request.AuthorExternalId
            };
        }
    }
}
