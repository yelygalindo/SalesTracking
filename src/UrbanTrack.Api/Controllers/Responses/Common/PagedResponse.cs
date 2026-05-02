using System.Collections.Generic;

namespace UrbanTrack.Api.Controllers.Responses.Pagination
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public PaginationResponse Pagination { get; set; }
    }
}