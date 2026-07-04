namespace SalesTracking.Application.UseCases.Projects.Results
{
    public sealed class DeleteProjectResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
