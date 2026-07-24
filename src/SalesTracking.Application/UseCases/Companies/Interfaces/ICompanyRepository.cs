using SalesTracking.Application.UseCases.Companies.Models;
using SalesTracking.Application.UseCases.Companies.Results;

namespace SalesTracking.Application.UseCases.Companies.Interfaces;

public interface ICompanyRepository
{
    Task<RegisterCompanyResult> RegisterAsync(RegisterCompany company);
}
