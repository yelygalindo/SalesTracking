using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using UrbanTrack.Api.Controllers.Requests.ProjectNotes;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ProjectNoteInputMapper
    {
        public static AddProjectNoteCommand ToApplication(
            this ProjectNoteRequest request,
            string projectExternalId)
        {
            return new AddProjectNoteCommand
            {
                ProjectExternalId = projectExternalId,
                Content = request.Content,
                AuthorExternalId = request.AuthorExternalId
            };
        }

        public static UpdateProjectNoteCommand ToApplication(
            this UpdateProjectNoteRequest request,
            string projectExternalId,
            string noteExternalId)
        {
            return new UpdateProjectNoteCommand
            {
                ProjectExternalId = projectExternalId,
                NoteExternalId = noteExternalId,
                Content = request.Content,
                UpdatedByUserExternalId = request.UpdatedByUserExternalId
            };
        }
    }
}