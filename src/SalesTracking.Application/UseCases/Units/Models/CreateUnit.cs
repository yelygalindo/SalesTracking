namespace SalesTracking.Application.UseCases.Units.Models
{
    public sealed class CreateUnit
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool AllowsDecimals { get; set; }
        public bool IsActive { get; set; }
    }
}