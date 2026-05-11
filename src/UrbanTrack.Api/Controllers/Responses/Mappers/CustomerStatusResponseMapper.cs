using SalesTracking.Domain.Enums;

namespace UrbanTrack.Api.Controllers.Responses.Mappers
{
    public static class CustomerStatusResponseMapper
    {
        public static string ToApiValue(this CustomerStatus status)
        {
            return status switch
            {
                CustomerStatus.Prospect => "prospect",
                CustomerStatus.Active => "active",
                CustomerStatus.Inactive => "inactive",
                _ => "unknown"
            };
        }
    }
}
