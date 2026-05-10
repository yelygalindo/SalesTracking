using System;

namespace UrbanTrack.Infrastructure.Sql.Entities
{
    public class AuthUserSqlEntity
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public CompanySqlEntity Company { get; set; }
    }
}