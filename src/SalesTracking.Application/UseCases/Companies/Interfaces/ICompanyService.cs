using SalesTracking.Application.UseCases.Companies.Comands;
using SalesTracking.Application.UseCases.Companies.Results;

namespace SalesTracking.Application.UseCases.Companies.Interfaces;

public interface ICompanyService
{
    Task<RegisterCompanyResult> RegisterAsync(RegisterCompanyCommand command);
}
