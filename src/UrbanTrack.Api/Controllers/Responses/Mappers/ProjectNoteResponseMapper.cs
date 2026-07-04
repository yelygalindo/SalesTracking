using SalesTracking.Application.UseCases.ProjectNotes.Results;
using UrbanTrack.Api.Controllers.Responses.Projects;

namespace UrbanTrack.Api.Controllers.Responses.Mappers
{
    public static class ProjectNoteResponseMapper
    {
        public static ProjectNoteResponse ToResponse(this ProjectNoteResult result)
        {
            return new ProjectNoteResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                Content = result.Content,
                CreatedBy = result.CreatedBy?.ToResponse(),
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedBy = result.UpdatedBy?.ToResponse(),
                UpdatedAtUtc = result.UpdatedAtUtc
            };
        }

        private static ProjectNoteUserResponse ToResponse(this ProjectNoteUserResult result)
        {
            return new ProjectNoteUserResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                Name = result.Name
            };
        }
    }
}
