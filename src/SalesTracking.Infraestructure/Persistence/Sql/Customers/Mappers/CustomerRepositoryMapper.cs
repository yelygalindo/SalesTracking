using SalesTracking.Domain.Entities;
using SalesTracking.Domain.Enums;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Rows;
using SalesTracking.Infrastructure.Persistence.Sql.Customers.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Customers.Mappers
{
    public static class CustomerRepositoryMapper
    {
        public static Customer ToDomain(this CustomerSummaryRow row)
        {
            return new Customer
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Name = row.Name,
                CompanyName = row.CompanyName,
                Phone = row.Phone,
                Email = row.Email,
                Status = (CustomerStatus)row.StatusId,
                Seller = new Seller() { Id = row.SellerId, ExternalId = row.ExternalSellerId, Name = row.SellerUserName },
                CreatedAtUtc = row.CreatedAt
            };
        }

        public static Customer ToDomain(this CustomerDetailRow row)
        {
            return new Customer
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Name = row.Name,
                CompanyName = row.CompanyName,
                Phone = row.Phone,
                Email = row.Email,
                Status = (CustomerStatus)row.StatusId,
                Seller = new Seller() { Id = row.SellerId, ExternalId = row.ExternalSellerId, Name = row.SellerUserName },
                Address = row.Address,
                Latitude = row.Latitude,
                Longitude = row.Longitude,
                CreatedAtUtc = row.CreatedAt
            };
        }

        public static CustomerNote ToDomain(this CustomerNoteRow row)
        {
            return new CustomerNote
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Text = row.Text,
                Author = new Author(row.AuthorId,row.AuthorExternalId,row.AuthorName),
                CreatedAtUtc = row.CreatedAt
            };
        }
    }
}
