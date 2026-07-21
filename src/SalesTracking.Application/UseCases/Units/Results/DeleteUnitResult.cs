namespace SalesTracking.Application.UseCases.Units.Results
{
    public sealed class DeleteUnitResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}