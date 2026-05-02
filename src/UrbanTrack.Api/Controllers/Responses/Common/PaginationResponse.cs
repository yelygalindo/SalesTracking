namespace UrbanTrack.Api.Controllers.Responses.Pagination
{
    public class PaginationResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}