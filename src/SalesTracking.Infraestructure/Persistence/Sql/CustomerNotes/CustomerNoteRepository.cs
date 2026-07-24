using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.CustomerNotes.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes
{
    public class CustomerNoteRepository : ICustomerNoteRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public CustomerNoteRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);
        private int CompanyId => _currentUser.CompanyId;
        public async Task<IReadOnlyList<CustomerNote>> GetNotesAsync(string customerExternalId)
        {
            using IDbConnection conn = CreateConnection();

            IEnumerable<CustomerNoteRow> notes = await conn.QueryAsync<CustomerNoteRow>(
                CustomerNoteRepositoryQueries.GetNotesByCustomerExternalId,
                new { CustomerExternalId = customerExternalId, CompanyId });

            return notes.Select(x => x.ToDomain()).ToList();
        }

        public async Task<ResponseCreateCustomerNote> AddNoteAsync(CreateCustomerNote note)
        {
            using IDbConnection conn = CreateConnection();
            conn.Open();

            using IDbTransaction transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerNoteRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { ExternalId = note.CustomerExternalId, CompanyId },
                    transaction);

                if (customerInternalId == null)
                {
                    transaction.Rollback();
                    return new ResponseCreateCustomerNote()
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Cliente no encontrado."
                    };
                }

                await conn.ExecuteAsync(
                    CustomerNoteRepositoryQueries.AddNote,
                    new
                    {
                        note.ExternalId,
                        CustomerId = customerInternalId.Value,
                        note.Text,
                        AuthorId = note.AuthorUserId,
                        CompanyId
                    },
                    transaction);

                await conn.ExecuteAsync(
                    CustomerNoteRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerInternalId.Value,
                        EventType = "CustomerNoteAdded",
                        Description = "Nota agregada al cliente.",
                        CreatedById = note.AuthorUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();
                return new ResponseCreateCustomerNote()
                {
                    Succeeded = true,
                    NotFound = false,
                    Message = "Nota agregada correctamente.",
                    CreateCustomerNote = note
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return new ResponseCreateCustomerNote()
                {
                    Succeeded = false,
                    NotFound = false,
                    Message = "Ocurrió un error al agregar la nota.",
                    CreateCustomerNote = note
                };
            }
        }
    }
}
