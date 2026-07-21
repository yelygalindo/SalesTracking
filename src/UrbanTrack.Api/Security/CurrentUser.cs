using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SalesTracking.Application.Common.Interfaces;

namespace UrbanTrack.Api.Security
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

        public int? UserId => GetIntClaim("sub") ?? GetIntClaim(ClaimTypes.NameIdentifier);

        public string? UserExternalId => GetClaim("userExternalId");

        public int? CompanyId => GetIntClaim("companyId");

        public string? Email => GetClaim("email") ?? GetClaim(ClaimTypes.Email);

        public string? Name => GetClaim("name") ?? GetClaim(ClaimTypes.Name) ?? Email;

        public IReadOnlyCollection<string> Roles => User?
            .FindAll(ClaimTypes.Role)
            .Concat(User.FindAll("role"))
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            ?? new List<string>();

        private string? GetClaim(string type) => User?.FindFirst(type)?.Value;

        private int? GetIntClaim(string type)
        {
            string? value = GetClaim(type);
            return int.TryParse(value, out int parsed) ? parsed : null;
        }
    }
}