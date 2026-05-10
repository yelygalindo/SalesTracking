namespace SalesTracking.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; } 

        public Company(int id, string externalId, string name)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
        }
    }
}
