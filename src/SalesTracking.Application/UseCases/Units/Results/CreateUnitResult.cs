namespace SalesTracking.Application.UseCases.Units.Results
{
    public sealed class CreateUnitResult
    {
        public bool Succeeded { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}