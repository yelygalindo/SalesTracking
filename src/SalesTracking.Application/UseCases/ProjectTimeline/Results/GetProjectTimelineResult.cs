using SalesTracking.Application.UseCases.ProjectTimeline.Models;

namespace SalesTracking.Application.UseCases.ProjectTimeline.Results
{
    public sealed class GetProjectTimelineResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProjectTimelinePagedList Timeline { get; set; } = new ProjectTimelinePagedList();
    }
}