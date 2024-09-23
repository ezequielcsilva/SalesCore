using Bogus;
using FluentAssertions;
using NSubstitute;
using SalesCore.Application.Abstractions.Data;
using SalesCore.Application.Orders.UpdateOrder;
using SalesCore.Domain.Orders;

namespace SalesCore.UnitTests.Application.Orders;

public class UpdateOrderCommandHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDbContext _dbContext;
    private readonly UpdateOrderCommandHandler _handler;
    private readonly Faker _faker;

    public UpdateOrderCommandHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _dbContext = Substitute.For<IDbContext>();
        _handler = new UpdateOrderCommandHandler(_orderRepository, _dbContext);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderIsNotFound()
    {
        // Arrange
        var request = new UpdateOrderCommand(
            new UpdateOrderRequest(_faker.Random.Guid(), [])
        );

        _orderRepository.GetByIdAsync(request.Order.OrderId, Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldCancelItem_WhenItemIsNotInRequest()
    {
        // Arrange
        var faker = new Faker();
        var existingItem = OrderItem.Create(faker.Random.Guid(), 2, 100m);
        var order = Order.Create(faker.Random.Guid(), faker.Random.Guid(), DateTime.UtcNow);

        order.AddItem(existingItem.ProductId, existingItem.Quantity, existingItem.Price);

        var request = new UpdateOrderCommand(
            new UpdateOrderRequest(order.Id, [])
        );

        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.OrderItems.First(x => x.ProductId == existingItem.ProductId).Cancelled.Should().BeTrue();
        _orderRepository.Received().Update(order);
    }

    [Fact]
    public async Task Handle_ShouldAddNewItem_WhenItemIsNotInOrder()
    {
        // Arrange
        var faker = new Faker();
        var newItem = new UpdateOrderItemRequest(faker.Random.Guid(), 1, 50m);
        var order = Order.Create(faker.Random.Guid(), faker.Random.Guid(), DateTime.UtcNow);

        var request = new UpdateOrderCommand(
            new UpdateOrderRequest(order.Id, [newItem])
        );

        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.OrderItems.Should().Contain(x => x.ProductId == newItem.ProductId);
        _orderRepository.Received().Update(order);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingItem_WhenItemExistsInOrder()
    {
        // Arrange
        var faker = new Faker();
        var existingItem = OrderItem.Create(faker.Random.Guid(), 2, 100m);
        var order = Order.Create(faker.Random.Guid(), faker.Random.Guid(), DateTime.UtcNow);
        order.AddItem(existingItem.ProductId, existingItem.Quantity, existingItem.Price);

        var updatedItem = new UpdateOrderItemRequest(existingItem.ProductId, 3, 150m);

        var request = new UpdateOrderCommand(
            new UpdateOrderRequest(order.Id, [updatedItem])
        );

        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedOrderItem = order.OrderItems.First(x => x.ProductId == updatedItem.ProductId);
        updatedOrderItem.Quantity.Should().Be(updatedItem.Quantity);
        updatedOrderItem.Price.Should().Be(updatedItem.Price);
        _orderRepository.Received().Update(order);
    }

    [Fact]
    public async Task Handle_ShouldNotCancelItem_WhenItemExistsInRequest()
    {
        // Arrange
        var faker = new Faker();
        var existingItem = OrderItem.Create(faker.Random.Guid(), 2, 100m);

        var order = Order.Create(faker.Random.Guid(), faker.Random.Guid(), DateTime.UtcNow);
        order.AddItem(existingItem.ProductId, existingItem.Quantity, existingItem.Price);

        var request = new UpdateOrderCommand(new UpdateOrderRequest(order.Id,
            [new UpdateOrderItemRequest(existingItem.ProductId, existingItem.Quantity, existingItem.Price)]));

        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.OrderItems.First(x => x.ProductId == existingItem.ProductId).Cancelled.Should().BeFalse();
        _orderRepository.Received().Update(order);
    }
}