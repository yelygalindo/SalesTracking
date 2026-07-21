namespace SalesTracking.Application.UseCases.Projects.Comands
{
    public sealed record DeleteProjectCommand(string ExternalId, int DeletedByUserId = 0);
}
