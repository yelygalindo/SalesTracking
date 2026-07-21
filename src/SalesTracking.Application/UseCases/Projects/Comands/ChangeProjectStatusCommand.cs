namespace SalesTracking.Application.UseCases.Projects.Comands
{
    public sealed class ChangeProjectStatusCommand
    {
        public string ExternalId { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public int ChangedByUserId { get; set; }
    }
}