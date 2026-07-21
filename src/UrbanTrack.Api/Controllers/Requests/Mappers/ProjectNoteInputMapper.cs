using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using UrbanTrack.Api.Controllers.Requests.ProjectNotes;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ProjectNoteInputMapper
    {
        public static AddProjectNoteCommand ToApplication(
            this ProjectNoteRequest request,
            string projectExternalId,
            int authorUserId)
        {
            return new AddProjectNoteCommand
            {
                ProjectExternalId = projectExternalId,
                Content = request.Content,
                AuthorUserId = authorUserId
            };
        }

        public static UpdateProjectNoteCommand ToApplication(
            this UpdateProjectNoteRequest request,
            string projectExternalId,
            string noteExternalId,
            int updatedByUserId)
        {
            return new UpdateProjectNoteCommand
            {
                ProjectExternalId = projectExternalId,
                NoteExternalId = noteExternalId,
                Content = request.Content,
                UpdatedByUserId = updatedByUserId
            };
        }
    }
}