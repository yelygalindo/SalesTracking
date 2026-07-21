namespace UrbanTrack.Api.Controllers.Requests.Units
{
    public sealed class UpdateUnitRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool AllowsDecimals { get; set; }
        public bool IsActive { get; set; } = true;
    }
}