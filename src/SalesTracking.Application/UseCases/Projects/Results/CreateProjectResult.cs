namespace SalesTracking.Application.UseCases.Projects.Results
{
    public sealed class CreateProjectResult
    {
        public bool Succeeded { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ProjectDetailResult? Project { get; set; }
    }
}
