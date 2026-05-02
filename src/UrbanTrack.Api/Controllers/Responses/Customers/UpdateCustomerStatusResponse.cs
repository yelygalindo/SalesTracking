namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public sealed record UpdateCustomerStatusResponse(
        string Id,
        string Status,
        string Message);
}