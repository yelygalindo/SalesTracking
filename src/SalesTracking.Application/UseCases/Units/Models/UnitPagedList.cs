using SalesTracking.Application.UseCases.Units.Results;

namespace SalesTracking.Application.UseCases.Units.Models
{
    public sealed class UnitPagedList
    {
        public IReadOnlyList<UnitResult> Items { get; set; } = new List<UnitResult>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}