namespace SalesTracking.Domain.Entities
{
    public class CustomerPagedList
    {
        public IReadOnlyList<Customer> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
