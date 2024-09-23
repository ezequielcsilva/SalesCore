using Bogus;
using FluentAssertions;
using SalesCore.Application.Orders.CreateOrder;
using SalesCore.Domain.Orders;

namespace SalesCore.IntegrationTests.Application.Orders;

public class OrdersTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Create_ShouldCreateOrder()
    {
        // Arrange
        var faker = new Faker();
        var orderItems = new List<CreateOrderItemRequest>
        {
            new(faker.Random.Guid(), 1, 100m),
            new(faker.Random.Guid(), 2, 150m)
        };

        var command = new CreateOrderCommand(new CreateOrderRequest(
            faker.Random.Guid(),
            faker.Random.Guid(),
            400m,
            orderItems,
            string.Empty,
            false,
            0m
        ));

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var createdOrder = DbContext.Set<Order>().FirstOrDefault(o => o.Id == result.Value.Id);

        createdOrder.Should().NotBeNull();
        createdOrder?.TotalAmount.Should().Be(400m);
        createdOrder?.OrderItems.Should().HaveCount(2);
        createdOrder?.OrderItems.ElementAt(0).ProductId.Should().Be(orderItems[0].ProductId);
        createdOrder?.OrderItems.ElementAt(0).Quantity.Should().Be(orderItems[0].Quantity);
        createdOrder?.OrderItems.ElementAt(1).ProductId.Should().Be(orderItems[1].ProductId);
        createdOrder?.OrderItems.ElementAt(1).Quantity.Should().Be(orderItems[1].Quantity);
    }
}