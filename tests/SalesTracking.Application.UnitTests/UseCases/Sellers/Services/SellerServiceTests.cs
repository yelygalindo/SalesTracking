using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Sellers.Comands;
using SalesTracking.Application.UseCases.Sellers.Interfaces;
using SalesTracking.Application.UseCases.Sellers.Results;
using SalesTracking.Application.UseCases.Sellers.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Sellers.Services;

public sealed class SellerServiceTests
{
    private readonly Mock<ISellerRepository> _repository = new();

    [Fact]
    public async Task GetAsync_WhenCompanyIsMissing_ShouldReturnEmptyWithoutQuerying()
    {
        SellerService service = new(_repository.Object);

        IReadOnlyList<SellerResult> result = await service.GetAsync(new GetSellersCommand(0));

        result.Should().BeEmpty();
        _repository.Verify(x => x.GetActiveAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_ShouldRestrictQueryToAuthenticatedCompany()
    {
        _repository.Setup(x => x.GetActiveAsync(7))
            .ReturnsAsync([new SellerResult { ExternalId = "seller-1" }]);
        SellerService service = new(_repository.Object);

        IReadOnlyList<SellerResult> result = await service.GetAsync(new GetSellersCommand(7));

        result.Should().ContainSingle();
        _repository.Verify(x => x.GetActiveAsync(7), Times.Once);
    }
}
