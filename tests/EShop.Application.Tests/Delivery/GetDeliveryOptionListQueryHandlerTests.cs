using EShop.Application.Contracts.Persistence;
using EShop.Application.Features.Delivery.Queries.GetDeliveryOptions;
using EShop.Domain.Entities;
using EShop.Domain.Enums;
using Moq;

namespace EShop.Application.Tests.Delivery;

public class GetDeliveryOptionListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenIncludeInactiveIsFalse_ReturnsActiveOptionsOnly()
    {
        var repo = new Mock<IDeliveryOptionRepository>();
        repo.Setup(r => r.ListActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeliveryOption>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Inside Dhaka",
                    Zone = DeliveryZone.InsideDhaka,
                    Cost = 80m,
                    IsActive = true,
                    SortOrder = 1
                }
            });

        var handler = new GetDeliveryOptionListQueryHandler(repo.Object);

        var result = await handler.Handle(new GetDeliveryOptionListQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("InsideDhaka", result[0].Zone);
        Assert.Equal(80m, result[0].Cost);
        repo.Verify(r => r.ListAllAsync(), Times.Never);
    }
}
