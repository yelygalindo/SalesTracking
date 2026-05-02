using UrbanTrack.Api.Controllers.Responses.Sellers;

namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public sealed record CustomerListItemResponse(
     string Id,
     string Name,
     string CompanyName,
     string Phone,
     string Email,
     string Status,
     string Address,
     decimal Latitude,
     decimal Longitude,
     SellerResponse Seller,
     DateTime RegisteredAt,
     DateTime? LastContactAt,
     DateTime? NextContactAt);
}
