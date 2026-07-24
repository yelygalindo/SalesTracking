using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Companies.Comands;
using SalesTracking.Application.UseCases.Companies.Interfaces;
using SalesTracking.Application.UseCases.Companies.Results;
using UrbanTrack.Api.Controllers.Requests.Companies;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Companies;

namespace UrbanTrack.Api.Controllers;

[ApiController]
[Route("api/companies")]
public sealed class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterCompanyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterCompanyResponse>> Register(
        [FromBody] RegisterCompanyRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return BadRequest(new MessageResponse { Message = "Las contraseñas no coinciden." });

        RegisterCompanyResult result = await _companyService.RegisterAsync(
            new RegisterCompanyCommand(
                request.CompanyName,
                request.AdminFullName,
                request.AdminEmail,
                request.Password));

        if (!result.Succeeded)
            return BadRequest(new MessageResponse { Message = result.Message });

        return StatusCode(StatusCodes.Status201Created, new RegisterCompanyResponse
        {
            CompanyExternalId = result.CompanyExternalId!,
            AdminUserExternalId = result.AdminUserExternalId!,
            Message = result.Message
        });
    }
}
