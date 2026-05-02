using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanTrack.Api.Controllers.Responses.Sellers
{
    public sealed record SellerResponse(
         string Id,
         string DisplayName);
}
