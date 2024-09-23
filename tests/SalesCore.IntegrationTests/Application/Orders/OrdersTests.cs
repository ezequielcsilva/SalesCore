using Bogus;
using FluentAssertions;
using SalesCore.Application.Orders.CreateOrder;
using SalesCore.Application.Orders.GetOrderById;
using SalesCore.Domain.Orders;

namespace SalesCore.IntegrationTests.Application.Orders;

public class OrdersTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Create_ShouldCreateOrder()
    {
        // Arrange
        var orderItems = new List<CreateOrderItemRequest>
        {
            new(_faker.Random.Guid(), 1, 100m),
            new(_faker.Random.Guid(), 2, 150m)
        };

        var command = new CreateOrderCommand(new CreateOrderRequest(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
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

    [Fact]
    public async Task Create_ShouldCreateOrder_WithVoucher()
    {
        // Arrange
        var orderItems = new List<CreateOrderItemRequest>
        {
            new(_faker.Random.Guid(), 1, 100m),
            new(_faker.Random.Guid(), 2, 150m)
        };

        var command = new CreateOrderCommand(new CreateOrderRequest(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
            400m,
            orderItems,
            "50-OFF",
            true,
            50m
        ));

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var createdOrder = DbContext.Set<Order>().FirstOrDefault(o => o.Id == result.Value.Id);

        createdOrder.Should().NotBeNull();
        createdOrder?.TotalAmount.Should().Be(350m);
        createdOrder?.OrderItems.Should().HaveCount(2);
        createdOrder?.OrderItems.ElementAt(0).ProductId.Should().Be(orderItems[0].ProductId);
        createdOrder?.OrderItems.ElementAt(0).Quantity.Should().Be(orderItems[0].Quantity);
        createdOrder?.OrderItems.ElementAt(1).ProductId.Should().Be(orderItems[1].ProductId);
        createdOrder?.OrderItems.ElementAt(1).Quantity.Should().Be(orderItems[1].Quantity);
        createdOrder?.Voucher?.Code.Should().Be("50-OFF");
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder()
    {
        // Arrange
        var orderItems = new List<CreateOrderItemRequest>
        {
            new(_faker.Random.Guid(), 1, 100m),
            new(_faker.Random.Guid(), 2, 150m)
        };

        var command = new CreateOrderCommand(new CreateOrderRequest(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
            400m,
            orderItems,
            string.Empty,
            false,
            0m
        ));

        // Act - Create order
        var createResult = await Sender.Send(command);

        // Assert - Ensure the order was created successfully
        createResult.IsSuccess.Should().BeTrue();
        var createdOrderId = createResult.Value.Id;

        // Act - Retrieve the created order by ID
        var getResult = await Sender.Send(new GetOrderByIdQuery(createdOrderId));

        // Assert - Ensure the retrieved order matches the created order
        getResult.IsSuccess.Should().BeTrue();
        var retrievedOrder = getResult.Value;

        retrievedOrder.Should().NotBeNull();
        retrievedOrder.TotalAmount.Should().Be(400m);
        retrievedOrder.Items.Should().HaveCount(2);
        retrievedOrder.Items.ElementAt(0).ProductId.Should().Be(orderItems[0].ProductId);
        retrievedOrder.Items.ElementAt(0).Quantity.Should().Be(orderItems[0].Quantity);
        retrievedOrder.Items.ElementAt(1).ProductId.Should().Be(orderItems[1].ProductId);
        retrievedOrder.Items.ElementAt(1).Quantity.Should().Be(orderItems[1].Quantity);
    }
}