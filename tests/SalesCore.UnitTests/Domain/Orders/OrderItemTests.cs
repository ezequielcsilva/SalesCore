using Bogus;
using FluentAssertions;
using SalesCore.Domain.Orders;

namespace SalesCore.UnitTests.Domain.Orders;

public class OrderItemTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_ShouldReturnValidOrderItem_WhenDataIsValid()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var quantity = _faker.Random.Int(1, 100);
        var price = _faker.Random.Decimal(1, 100);

        // Act
        var orderItem = OrderItem.Create(productId, quantity, price);

        // Assert
        orderItem.ProductId.Should().Be(productId);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.Price.Should().Be(price);
        orderItem.IsCancelled.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ShouldThrowException_WhenQuantityIsZeroOrNegative(int invalidQuantity)
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var price = _faker.Random.Int(1, 100);

        // Act
        Action act = () => OrderItem.Create(productId, invalidQuantity, price);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Quantity must be greater than zero*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPriceIsNegative()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var quantity = _faker.Random.Int(1, 100);
        var invalidPrice = -1m;

        // Act
        Action act = () => OrderItem.Create(productId, quantity, invalidPrice);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Price cannot be negative*");
    }

    [Fact]
    public void GetAmount_ShouldReturnCorrectAmount_WhenValidQuantityAndPrice()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var quantity = 3;
        var price = 10m;

        var orderItem = OrderItem.Create(productId, quantity, price);

        // Act
        var amount = orderItem.GetAmount();

        // Assert
        amount.Should().Be(quantity * price);
    }
}