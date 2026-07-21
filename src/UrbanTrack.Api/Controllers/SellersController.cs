using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Sellers.Comands;
using SalesTracking.Application.UseCases.Sellers.Interfaces;
using SalesTracking.Application.UseCases.Sellers.Results;
using UrbanTrack.Api.Controllers.Responses.Sellers;

namespace UrbanTrack.Api.Controllers;

[ApiController]
[Route("api/sellers")]
public sealed class SellersController : ControllerBase
{
    private readonly ISellerService _service;
    private readonly ICurrentUser _currentUser;

    public SellersController(ISellerService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SellerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SellerResponse>>> Get()
    {
        IReadOnlyList<SellerResult> sellers = await _service.GetAsync(
            new GetSellersCommand(_currentUser.CompanyId.GetValueOrDefault()));

        return Ok(sellers.Select(seller => new SellerResponse(
            seller.ExternalId,
            seller.DisplayName,
            seller.Email)).ToList());
    }
}
