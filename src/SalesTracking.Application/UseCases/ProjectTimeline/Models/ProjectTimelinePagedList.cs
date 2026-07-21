using SalesTracking.Application.UseCases.ProjectTimeline.Results;

namespace SalesTracking.Application.UseCases.ProjectTimeline.Models
{
    public sealed class ProjectTimelinePagedList
    {
        public IReadOnlyList<ProjectTimelineResult> Items { get; set; } = new List<ProjectTimelineResult>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}