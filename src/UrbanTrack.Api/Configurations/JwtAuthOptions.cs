using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanTrack.Api.Configurations
{
    public class JwtAuthOptions
    {
        public string TokenExpirationToleranceInSeconds { get; set; }
        public bool Enabled { get; set; }
        public string SchemeName { get; set; }
    }
}
