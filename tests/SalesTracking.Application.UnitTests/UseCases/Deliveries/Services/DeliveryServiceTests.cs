using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Results;
using SalesTracking.Application.UseCases.Deliveries.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Deliveries.Services;

public sealed class DeliveryServiceTests
{
    private readonly Mock<IDeliveryRepository> _repository = new();

    [Fact]
    public async Task GetAsync_ShouldNormalizePagination()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<GetDeliveriesCommand>()))
            .ReturnsAsync(new Application.UseCases.Deliveries.Models.DeliveryPagedList());
        DeliveryService service = new(_repository.Object);

        await service.GetAsync(new GetDeliveriesCommand(0, 500));

        _repository.Verify(x => x.GetAsync(It.Is<GetDeliveriesCommand>(command =>
            command.Page == 1 && command.PageSize == 100)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenItemsAreMissing_ShouldReturnValidationError()
    {
        DeliveryService service = new(_repository.Object);

        CreateDeliveryResult result = await service.CreateAsync(new CreateDeliveryCommand
        {
            ProjectExternalId = "project-1", SellerExternalId = "seller-1", Items = []
        });

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("La entrega debe tener al menos un item.");
        _repository.Verify(x => x.CreateAsync(It.IsAny<Application.UseCases.Deliveries.Models.CreateDelivery>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmReceiptAsync_ShouldTrimAndGroupRepeatedItems()
    {
        _repository.Setup(x => x.ConfirmReceiptAsync(It.IsAny<ConfirmDeliveryReceiptCommand>()))
            .ReturnsAsync(new ConfirmDeliveryReceiptResult { Succeeded = true });
        DeliveryService service = new(_repository.Object);

        await service.ConfirmReceiptAsync(new ConfirmDeliveryReceiptCommand
        {
            DeliveryExternalId = " delivery-1 ",
            Notes = " received ",
            Items =
            [
                new() { DeliveryItemExternalId = " item-1 ", ReceivedQuantity = 2 },
                new() { DeliveryItemExternalId = "item-1", ReceivedQuantity = 3 }
            ]
        });

        _repository.Verify(x => x.ConfirmReceiptAsync(It.Is<ConfirmDeliveryReceiptCommand>(command =>
            command.DeliveryExternalId == "delivery-1" && command.Notes == "received" &&
            command.Items.Count == 1 && command.Items.Single().ReceivedQuantity == 5)), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusAsync_WhenStatusIsUnknown_ShouldNotCallRepository()
    {
        DeliveryService service = new(_repository.Object);

        ChangeDeliveryStatusResult result = await service.ChangeStatusAsync(new ChangeDeliveryStatusCommand
        {
            ExternalId = "delivery-1", StatusId = 99
        });

        result.Succeeded.Should().BeFalse();
        _repository.Verify(x => x.ChangeStatusAsync(It.IsAny<ChangeDeliveryStatusCommand>()), Times.Never);
    }
}
