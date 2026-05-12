using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Mappers
{
    public static class CustomerNoteRepositoryMapper
    {
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
