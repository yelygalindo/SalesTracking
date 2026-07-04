using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracking.Application.UseCases.Projects.Comands
{
    public sealed record GetProjectsCommand(
    string? Status,
    string? CustomerId,
    string? SellerId,
    int Page,
    int PageSize
);
}
