namespace UrbanTrack.Api.Controllers.Responses.PaginationResponses
{
    public sealed record PaginationResponse(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
}
