using Bogus;
using FluentAssertions;
using SalesCore.Domain.Orders;

namespace SalesCore.UnitTests.Domain.Orders;

public class OrderTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_ShouldReturnValidOrder_WhenDataIsValid()
    {
        // Arrange
        var customerId = _faker.Random.Guid();
        var branchId = _faker.Random.Guid();
        var utcNow = DateTime.UtcNow;

        var orderItemFaker = new Faker<OrderItem>()
            .CustomInstantiator(f => OrderItem.Create(
                f.Random.Guid(),
                f.Random.Int(1, 100),
                f.Random.Decimal(1, 100)
            ));

        var orderItems = orderItemFaker.Generate(5);

        var discount = 10m;

        // Act
        var order = Order.Create(customerId, branchId, utcNow, orderItems, discount);

        // Assert
        order.CustomerId.Should().Be(customerId);
        order.BranchId.Should().Be(branchId);
        order.DateAdded.Should().Be(utcNow);
        order.Discount.Should().Be(discount);
        order.OrderItems.Should().BeEquivalentTo(orderItems);
    }

    [Fact]
    public void TotalAmount_ShouldBeCorrect_AfterCreationWithNonCancelledItems()
    {
        // Arrange
        var customerId = _faker.Random.Guid();
        var branchId = _faker.Random.Guid();
        var utcNow = DateTime.UtcNow;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create( _faker.Random.Guid(), 2, 50m),
            OrderItem.Create( _faker.Random.Guid(), 1, 100m)
        };
        var discount = 20m;

        // Act
        var order = Order.Create(customerId, branchId, utcNow, orderItems, discount);

        // Assert
        var expectedTotalAmount = orderItems.Sum(i => i.GetAmount()) - discount;
        order.TotalAmount.Should().Be(expectedTotalAmount);
    }

    [Fact]
    public void TotalAmount_ShouldApplyDiscountToNonCancelledItems()
    {
        // Arrange
        var customerId = _faker.Random.Guid();
        var branchId = _faker.Random.Guid();
        var utcNow = DateTime.UtcNow;
        var activeItem = OrderItem.Create(_faker.Random.Guid(), 2, 50m);
        var cancelledItem = OrderItem.Create(_faker.Random.Guid(), 1, 100m);
        cancelledItem.Cancel();
        var orderItems = new List<OrderItem> { activeItem, cancelledItem };
        var discount = 20m;

        // Act
        var order = Order.Create(customerId, branchId, utcNow, orderItems, discount);

        // Assert
        var expectedTotalAmount = activeItem.GetAmount() - discount;
        order.TotalAmount.Should().Be(expectedTotalAmount);
    }

    [Fact]
    public void CancelledItemsAmount_ShouldBeCorrect_WhenOrderHasCancelledItems()
    {
        // Arrange
        var customerId = _faker.Random.Guid();
        var branchId = _faker.Random.Guid();
        var utcNow = DateTime.UtcNow;
        var activeItem = OrderItem.Create(_faker.Random.Guid(), 2, 50m);
        var cancelledItem = OrderItem.Create(_faker.Random.Guid(), 1, 100m);
        cancelledItem.Cancel();
        var orderItems = new List<OrderItem> { activeItem, cancelledItem };

        // Act
        var order = Order.Create(customerId, branchId, utcNow, orderItems);

        // Assert
        order.CancelledItemsAmount.Should().Be(cancelledItem.GetAmount());
    }

    [Fact]
    public void TotalAmount_ShouldNotBeNegative_WhenDiscountExceedsActiveItemAmounts()
    {
        // Arrange
        var customerId = _faker.Random.Guid();
        var branchId = _faker.Random.Guid();
        var utcNow = DateTime.UtcNow;
        var activeItem = OrderItem.Create(_faker.Random.Guid(), 1, 50m);
        var orderItems = new List<OrderItem> { activeItem };
        var discount = 100m;

        // Act
        var order = Order.Create(customerId, branchId, utcNow, orderItems, discount);

        // Assert
        order.TotalAmount.Should().Be(0);
    }
}