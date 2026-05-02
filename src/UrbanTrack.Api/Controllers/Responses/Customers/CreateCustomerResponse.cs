
namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public sealed record CreateCustomerResponse(
        string Id,
        string Name,
        string CompanyName,
        string Phone,
        string Email,
        string Status,
        DateTime CreatedAt);
}
