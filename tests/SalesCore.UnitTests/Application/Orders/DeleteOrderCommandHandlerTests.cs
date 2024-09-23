using Bogus;
using FluentAssertions;
using NSubstitute;
using SalesCore.Application.Orders.DeleteOrder;
using SalesCore.Domain.Orders;

namespace SalesCore.UnitTests.Application.Orders;

public class DeleteOrderCommandHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly DeleteOrderCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteOrderCommandHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _handler = new DeleteOrderCommandHandler(_orderRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderIsNotFound()
    {
        // Arrange
        var orderId = _faker.Random.Guid();

        _orderRepository.GetByIdAsync(orderId, Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var command = new DeleteOrderCommand(orderId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldDeleteOrder_WhenOrderIsFound()
    {
        // Arrange
        var orderId = _faker.Random.Guid();
        var order = Order.Create(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow);

        _orderRepository.GetByIdAsync(orderId, Arg.Any<CancellationToken>())
            .Returns(order);

        var command = new DeleteOrderCommand(orderId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepository.Received(1).Delete(order);
    }
}