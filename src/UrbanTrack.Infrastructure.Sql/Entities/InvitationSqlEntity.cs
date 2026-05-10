using System;

namespace UrbanTrack.Infrastructure.Sql.Entities
{
    public class InvitationSqlEntity
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string InvitedBy { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}