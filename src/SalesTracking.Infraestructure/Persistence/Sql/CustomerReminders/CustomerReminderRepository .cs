using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.CustomerReminders.Interfaces;
using SalesTracking.Application.UseCases.CustomerReminders.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders
{
    public class CustomerReminderRepository : ICustomerReminderRepository
    {

        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public CustomerReminderRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);
        private int CompanyId => _currentUser.CompanyId;

        public async Task<IReadOnlyList<CustomerReminder>> GetRemindersAsync(
            string customerExternalId,
            bool? completed = null)
        {
            using IDbConnection conn = CreateConnection();

            IEnumerable<CustomerReminderRow> reminders =
                await conn.QueryAsync<CustomerReminderRow>(
                    CustomerReminderRepositoryQueries.GetRemindersByCustomerExternalId,
                    new
                    {
                        CustomerExternalId = customerExternalId,
                        CompanyId,
                        Completed = completed
                    });

            return reminders.Select(x => x.ToDomain()).ToList();
        }

        public async Task<CreateCustomerReminder> CreateReminderAsync(CreateCustomerReminder reminder)
        {
            using IDbConnection conn = CreateConnection();
            conn.Open();

            using IDbTransaction transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerReminderRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { ExternalId = reminder.CustomerExternalId, CompanyId },
                    transaction);

                if (customerInternalId == null)
                {
                    transaction.Rollback();

                    reminder.Succeeded = false;
                    reminder.NotFound = true;
                    reminder.Message = "Cliente no encontrado.";

                    return reminder;
                }

                int? assignedToInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerReminderRepositoryQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = reminder.AssignedToId, CompanyId },
                    transaction);

                if (assignedToInternalId == null)
                {
                    transaction.Rollback();

                    reminder.Succeeded = false;
                    reminder.Message = "Usuario asignado no encontrado o inactivo.";

                    return reminder;
                }

                await conn.ExecuteAsync(
                    CustomerReminderRepositoryQueries.CreateReminder,
                    new
                    {
                        reminder.ExternalId,
                        CustomerId = customerInternalId.Value,
                        reminder.Text,
                        reminder.ReminderAtUtc,
                        AssignedToId = assignedToInternalId.Value,
                        CompanyId
                    },
                    transaction);

                await conn.ExecuteAsync(
                    CustomerReminderRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerInternalId.Value,
                        EventType = "CustomerReminderCreated",
                        Description = "Recordatorio creado.",
                        CreatedById = reminder.CreatedByUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();

                reminder.Succeeded = true;
                reminder.Message = "Recordatorio creado correctamente.";

                return reminder;
            }
            catch
            {
                transaction.Rollback();

                reminder.Succeeded = false;
                reminder.Message = "Ocurrió un error al crear el recordatorio.";

                return reminder;
            }
        }

        public async Task<bool> CompleteReminderAsync(string customerExternalId, string reminderExternalId, int completedByUserId)
        {
            using IDbConnection conn = CreateConnection();
            conn.Open();

            using IDbTransaction transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerReminderRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { ExternalId = customerExternalId, CompanyId },
                    transaction);

                if (customerInternalId == null)
                {
                    transaction.Rollback();
                    return false;
                }

                int rows = await conn.ExecuteAsync(
                    CustomerReminderRepositoryQueries.CompleteReminder,
                    new
                    {
                        CustomerExternalId = customerExternalId,
                        ReminderExternalId = reminderExternalId,
                        CompanyId
                    },
                    transaction);

                if (rows == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                await conn.ExecuteAsync(
                    CustomerReminderRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerInternalId.Value,
                        EventType = "CustomerReminderCompleted",
                        Description = "Recordatorio marcado como completado.",
                        CreatedById = completedByUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
