namespace UrbanTrack.Api.Controllers.Requests.Units
{
    public sealed class GetUnitsRequest
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}