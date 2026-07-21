using SalesTracking.Application.UseCases.CustomerTimeline.Comands;
using SalesTracking.Application.UseCases.CustomerTimeline.Models;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;
using UrbanTrack.Api.Controllers.Requests.CustomerTimeline;
using UrbanTrack.Api.Controllers.Responses.CustomerTimeline;
using UrbanTrack.Api.Controllers.Responses.Pagination;

namespace UrbanTrack.Api.Controllers.Requests.Mappers;

public static class CustomerTimelineApiMapper
{
    public static GetCustomerTimelineCommand ToApplication(
        this GetCustomerTimelineRequest request,
        string customerExternalId) =>
        new(customerExternalId, request.Page, request.PageSize);

    public static PagedResponse<CustomerTimelineResponse> ToResponse(this CustomerTimelinePagedList timeline) => new()
    {
        Items = timeline.Items.Select(ToResponse).ToList(),
        Pagination = new PaginationResponse
        {
            Page = timeline.Page,
            PageSize = timeline.PageSize,
            TotalItems = timeline.TotalItems,
            TotalPages = timeline.TotalPages
        }
    };

    private static CustomerTimelineResponse ToResponse(CustomerTimelineResult result) => new()
    {
        ExternalId = result.ExternalId,
        EventType = result.EventType,
        Description = result.Description,
        CreatedAtUtc = result.CreatedAtUtc,
        CreatedBy = result.CreatedBy == null
            ? null
            : new CustomerTimelineUserResponse
            {
                ExternalId = result.CreatedBy.ExternalId,
                Name = result.CreatedBy.Name
            }
    };
}
