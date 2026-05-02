using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanTrack.Api.Configurations
{
    public class AuthTokenOptions
    {
        public string Token {  get; set; }
        public bool ValidateAuthToken {  get; set; }
    }
}
