using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.CustomerTimeline.Comands;
using SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;
using SalesTracking.Application.UseCases.CustomerTimeline.Models;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerTimeline.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerTimeline;

public sealed class CustomerTimelineRepository : ICustomerTimelineRepository
{
    private readonly DatabaseSettings _settings;
    private readonly ICurrentUser _currentUser;

    public CustomerTimelineRepository(IOptions<DatabaseSettings> settings, ICurrentUser currentUser)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _currentUser = currentUser;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_settings.ConnectionString);
    private int CompanyId => _currentUser.CompanyId;

    public async Task<GetCustomerTimelineResult> GetAsync(GetCustomerTimelineCommand command)
    {
        using IDbConnection connection = CreateConnection();
        int? customerId = await connection.QueryFirstOrDefaultAsync<int?>(
            CustomerTimelineQueries.GetCustomerId,
            new { command.CustomerExternalId, CompanyId });

        if (customerId == null)
        {
            return new GetCustomerTimelineResult
            {
                Succeeded = false,
                NotFound = true,
                Message = "Cliente no encontrado."
            };
        }

        List<CustomerTimelineRow> rows = (await connection.QueryAsync<CustomerTimelineRow>(
            CustomerTimelineQueries.GetTimeline,
            new
            {
                CustomerId = customerId.Value,
                Offset = (command.Page - 1) * command.PageSize,
                command.PageSize,
                CompanyId
            })).ToList();

        int totalItems = rows.FirstOrDefault()?.TotalCount ?? 0;
        return new GetCustomerTimelineResult
        {
            Succeeded = true,
            Timeline = new CustomerTimelinePagedList
            {
                Items = rows.Select(ToResult).ToList(),
                Page = command.Page,
                PageSize = command.PageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)command.PageSize)
            }
        };
    }

    private static CustomerTimelineResult ToResult(CustomerTimelineRow row) => new()
    {
        ExternalId = row.ExternalId,
        EventType = row.EventType,
        Description = row.Description,
        CreatedAtUtc = row.CreatedAtUtc,
        CreatedBy = string.IsNullOrWhiteSpace(row.CreatedByExternalId) && string.IsNullOrWhiteSpace(row.CreatedByName)
            ? null
            : new CustomerTimelineUserResult
            {
                ExternalId = row.CreatedByExternalId ?? string.Empty,
                Name = row.CreatedByName
            }
    };
}
