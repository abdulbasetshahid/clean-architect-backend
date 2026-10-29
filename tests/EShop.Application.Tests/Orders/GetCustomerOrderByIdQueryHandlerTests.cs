using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Application.Features.Orders.Queries.GetCustomerOrders;
using EShop.Domain.Entities;
using Moq;

namespace EShop.Application.Tests.Orders;

public class GetCustomerOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_OrderNotOwnedByCaller_ThrowsNotFound()
    {
        var orderId = Guid.NewGuid();
        var orders = new Mock<IOrderRepository>();
        orders.Setup(r => r.GetByIdWithDetailsForCustomerAsync(orderId, "uid-b", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new GetCustomerOrderByIdQueryHandler(orders.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetCustomerOrderByIdQuery(orderId, "uid-b"), CancellationToken.None));
        orders.Verify(r => r.GetByIdWithDetailsForCustomerAsync(orderId, "uid-b", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MissingUid_ThrowsUnauthorized_WithoutQuerying()
    {
        var orders = new Mock<IOrderRepository>();
        var handler = new GetCustomerOrderByIdQueryHandler(orders.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new GetCustomerOrderByIdQuery(Guid.NewGuid(), ""), CancellationToken.None));
        orders.VerifyNoOtherCalls();
    }
}
