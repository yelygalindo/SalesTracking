using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Mappers
{
    public static class CustomerStatusMapper
    {
        public static int ToValue(this CustomerStatus status)
        {
            return (int)status;
        }

        public static string ToLabel(this CustomerStatus status)
        {
            return status switch
            {
                CustomerStatus.Prospect => "Prospecto",
                CustomerStatus.Active => "Activo",
                CustomerStatus.Inactive => "Inactivo",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }

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
