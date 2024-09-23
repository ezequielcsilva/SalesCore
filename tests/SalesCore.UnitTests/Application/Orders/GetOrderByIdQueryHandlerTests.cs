using Bogus;
using FluentAssertions;
using NSubstitute;
using SalesCore.Application.Orders.GetOrderById;
using SalesCore.Domain.Orders;

namespace SalesCore.UnitTests.Application.Orders;

public class GetOrderByIdQueryHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly GetOrderByIdQueryHandler _handler;
    private readonly Faker _faker;

    public GetOrderByIdQueryHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _handler = new GetOrderByIdQueryHandler(_orderRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenOrderIsFound()
    {
        // Arrange
        var customerId = _faker.Random.Guid();
        var branchId = _faker.Random.Guid();
        var dateAdded = DateTime.UtcNow;

        var order = Order.Create(customerId, branchId, dateAdded, false, 0);
        order.AddItem(_faker.Random.Guid(), 2, 50m);
        order.AddItem(_faker.Random.Guid(), 1, 100m);

        _orderRepository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var query = new GetOrderByIdQuery(order.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.OrderId.Should().Be(order.Id);
        result.Value.TotalAmount.Should().Be(order.OrderItems.Sum(oi => oi.GetAmount()));
        result.Value.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderIsNotFound()
    {
        // Arrange
        var faker = new Faker();
        var orderId = faker.Random.Guid();

        _orderRepository.GetByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);

        var query = new GetOrderByIdQuery(orderId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e == OrderErrors.NotFound);
    }
}