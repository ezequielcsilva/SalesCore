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
        orderItem.Cancelled.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var invalidQuantity = 0;
        var price = _faker.Random.Decimal(1, 100);

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

    [Fact]
    public void Cancel_ShouldSetCancelledToTrue_WhenCalled()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var quantity = _faker.Random.Int(1, 100);
        var price = _faker.Random.Decimal(1, 100);
        var orderItem = OrderItem.Create(productId, quantity, price);

        // Act
        orderItem.Cancel();

        // Assert
        orderItem.Cancelled.Should().BeTrue();
    }

    [Fact]
    public void Update_ShouldUpdateQuantityAndPrice_WhenCalledWithValidValues()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var originalQuantity = _faker.Random.Int(1, 100);
        var originalPrice = _faker.Random.Decimal(1, 100);
        var orderItem = OrderItem.Create(productId, originalQuantity, originalPrice);

        var newQuantity = _faker.Random.Int(1, 100);
        var newPrice = _faker.Random.Decimal(1, 100);

        // Act
        orderItem.Update(newQuantity, newPrice);

        // Assert
        orderItem.Quantity.Should().Be(newQuantity);
        orderItem.Price.Should().Be(newPrice);
        orderItem.Cancelled.Should().BeFalse();
    }

    [Fact]
    public void Update_ShouldNotThrow_WhenQuantityIsValid()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var orderItem = OrderItem.Create(productId, 1, 10);

        var newQuantity = _faker.Random.Int(1, 100);
        var newPrice = _faker.Random.Decimal(1, 100);

        // Act
        var act = () => orderItem.Update(newQuantity, newPrice);

        // Assert
        act.Should().NotThrow();
    }
}