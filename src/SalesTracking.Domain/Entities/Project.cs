using SalesTracking.Domain.Enums;

namespace SalesTracking.Domain.Entities
{
    public sealed class Project
    {
        public int Id { get; private set; }
        public string ExternalId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string CustomerExternalId { get; private set; }
        public string SellerExternalId { get; private set; }
        public ProjectStatus Status { get; private set; }
        public decimal? EstimatedAmount { get; private set; }
        public DateTime? StartDateUtc { get; private set; }
        public DateTime? ExpectedCloseDateUtc { get; private set; }

        private Project(
            string externalId,
            string name,
            string? description,
            string customerExternalId,
            string sellerExternalId,
            ProjectStatus status,
            decimal? estimatedAmount,
            DateTime? startDateUtc,
            DateTime? expectedCloseDateUtc)
        {
            ExternalId = externalId;
            Name = name;
            Description = description;
            CustomerExternalId = customerExternalId;
            SellerExternalId = sellerExternalId;
            Status = status;
            EstimatedAmount = estimatedAmount;
            StartDateUtc = startDateUtc;
            ExpectedCloseDateUtc = expectedCloseDateUtc;
        }

        public static Project Create(
            string externalId,
            string name,
            string? description,
            string customerExternalId,
            string sellerExternalId,
            decimal? estimatedAmount,
            DateTime? startDateUtc,
            DateTime? expectedCloseDateUtc)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("External id is required.", nameof(externalId));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(customerExternalId))
                throw new ArgumentException("Customer id is required.", nameof(customerExternalId));

            if (string.IsNullOrWhiteSpace(sellerExternalId))
                throw new ArgumentException("Seller id is required.", nameof(sellerExternalId));

            return new Project(
                externalId,
                name.Trim(),
                description?.Trim(),
                customerExternalId,
                sellerExternalId,
                ProjectStatus.Active,
                estimatedAmount,
                startDateUtc,
                expectedCloseDateUtc);
        }
    }
}
