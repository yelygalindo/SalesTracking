using SalesTracking.Application.UseCases.CustomerNotes.Results;
using SalesTracking.Domain.Entities;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.CustomerNotes;

namespace UrbanTrack.Api.Controllers.Responses.Mappers
{
    public static class CustomerNoteResponseMapper
    {
        public static CustomerNoteResponse ToResponse(this CustomerNote note)
        {
            return new CustomerNoteResponse
            {
                Id = note.Id,
                ExternalId = note.ExternalId,
                Text = note.Text,
                Author = new UserResponse(note.Author.Id, note.Author.ExternalId, note.Author.Name),
                CreatedAt = note.CreatedAtUtc
            };
        }

        public static IdMessageResponse ToResponse(this AddCustomerNoteResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}
