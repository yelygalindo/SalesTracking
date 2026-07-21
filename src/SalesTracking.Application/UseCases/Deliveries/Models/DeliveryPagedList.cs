using SalesTracking.Application.UseCases.Deliveries.Results;

namespace SalesTracking.Application.UseCases.Deliveries.Models
{
    public sealed class DeliveryPagedList
    {
        public IReadOnlyList<DeliveryResult> Items { get; set; } = new List<DeliveryResult>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}