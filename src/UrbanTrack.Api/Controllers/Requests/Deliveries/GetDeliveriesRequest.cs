namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public sealed class GetDeliveriesRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
