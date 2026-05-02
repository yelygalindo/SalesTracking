using System;
using System.Collections.Generic;

namespace UrbanTrack.Api.Controllers.Responses.Projects
{
    public class ProjectDetailResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string SellerId { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<ProjectProductResponse> Products { get; set; } = new List<ProjectProductResponse>();
    }

    public class ProjectProductResponse
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}