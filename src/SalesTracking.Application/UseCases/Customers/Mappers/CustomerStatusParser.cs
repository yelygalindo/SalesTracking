using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Mappers
{
    public static class CustomerStatusParser
    {
        public static bool TryParse(string? value, out CustomerStatus status)
        {
            status = default;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            status = value.Trim().ToLowerInvariant() switch
            {
                "prospect" => CustomerStatus.Prospect,
                "active" => CustomerStatus.Active,
                "inactive" => CustomerStatus.Inactive,
                _ => default
            };

            return status != default;
        }
    }
}
