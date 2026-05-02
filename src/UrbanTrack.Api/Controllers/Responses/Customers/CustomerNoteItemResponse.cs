using UrbanTrack.Api.Controllers.Responses.Sellers;

namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public sealed record CustomerNoteItemResponse(
     string Id,
     string Content,
     DateTime CreatedAt,
     SellerResponse CreatedBy);
}
