using UrbanTrack.Api.Controllers.Responses.PaginationResponses;

namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public sealed record CustomersListResponse(
        IReadOnlyCollection<CustomerListItemResponse> Items,
        PaginationResponse Pagination);
}