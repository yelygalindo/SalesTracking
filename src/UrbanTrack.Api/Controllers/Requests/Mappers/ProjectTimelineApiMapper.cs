using SalesTracking.Application.UseCases.ProjectTimeline.Comands;
using SalesTracking.Application.UseCases.ProjectTimeline.Models;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;
using UrbanTrack.Api.Controllers.Requests.ProjectTimeline;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.ProjectTimeline;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ProjectTimelineApiMapper
    {
        public static GetProjectTimelineCommand ToApplication(
            this GetProjectTimelineRequest request,
            string projectExternalId)
        {
            return new GetProjectTimelineCommand(
                projectExternalId,
                request.Page,
                request.PageSize);
        }

        public static ProjectTimelineResponse ToResponse(this ProjectTimelineResult result)
        {
            return new ProjectTimelineResponse
            {
                ExternalId = result.ExternalId,
                EventTypeId = result.EventTypeId,
                EventTypeName = result.EventTypeName,
                Title = result.Title,
                Description = result.Description,
                OccurredAtUtc = result.OccurredAtUtc,
                CreatedBy = result.CreatedBy?.ToResponse(),
                RelatedEntityType = result.RelatedEntityType,
                RelatedEntityId = result.RelatedEntityId,
                MetadataJson = result.MetadataJson
            };
        }

        public static PagedResponse<ProjectTimelineResponse> ToResponse(this ProjectTimelinePagedList result)
        {
            return new PagedResponse<ProjectTimelineResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = new PaginationResponse
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                }
            };
        }

        private static ProjectTimelineUserResponse ToResponse(this ProjectTimelineUserResult result)
        {
            return new ProjectTimelineUserResponse
            {
                ExternalId = result.ExternalId,
                Name = result.Name
            };
        }
    }
}