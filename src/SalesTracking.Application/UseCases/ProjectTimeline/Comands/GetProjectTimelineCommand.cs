namespace SalesTracking.Application.UseCases.ProjectTimeline.Comands
{
    public sealed record GetProjectTimelineCommand(
        string ProjectExternalId,
        int Page,
        int PageSize);
}