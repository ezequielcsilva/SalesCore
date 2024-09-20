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
        var discount = _faker.Random.Decimal(0, 100);

        // Act
        var order = Order.Create(customerId, branchId, utcNow, discount);

        // Assert
        order.CustomerId.Should().Be(customerId);
        order.BranchId.Should().Be(branchId);
        order.DateAdded.Should().BeCloseTo(utcNow, TimeSpan.FromSeconds(1));
        order.Discount.Should().Be(discount);
        order.TotalAmount.Should().Be(0);
        order.CancelledItemsAmount.Should().Be(0);
    }

    [Fact]
    public void AddItem_ShouldAddNewItem_WhenProductDoesNotExist()
    {
        // Arrange
        var order = Order.Create(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow);
        var productId = _faker.Random.Guid();
        var quantity = _faker.Random.Int(1, 100);
        var price = _faker.Random.Decimal(1, 100);

        // Act
        order.AddItem(productId, quantity, price);

        // Assert
        order.OrderItems.Should().ContainSingle();
        var addedItem = order.OrderItems.First();
        addedItem.ProductId.Should().Be(productId);
        addedItem.Quantity.Should().Be(quantity);
        addedItem.Price.Should().Be(price);
    }

    [Fact]
    public void AddItem_ShouldUpdateExistingItem_WhenProductAlreadyExists()
    {
        // Arrange
        var order = Order.Create(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow);
        var productId = _faker.Random.Guid();
        var initialQuantity = _faker.Random.Int(1, 10);
        var initialPrice = _faker.Random.Decimal(1, 50);
        order.AddItem(productId, initialQuantity, initialPrice);

        var newQuantity = _faker.Random.Int(1, 100);
        var newPrice = _faker.Random.Decimal(1, 100);

        // Act
        order.AddItem(productId, newQuantity, newPrice);

        // Assert
        order.OrderItems.Should().ContainSingle();
        var updatedItem = order.OrderItems.First();
        updatedItem.Quantity.Should().Be(newQuantity);
        updatedItem.Price.Should().Be(newPrice);
    }

    [Fact]
    public void CancelItem_ShouldSetItemAsCancelled_WhenProductExists()
    {
        // Arrange
        var order = Order.Create(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow);
        var productId = _faker.Random.Guid();
        order.AddItem(productId, 1, 10);

        // Act
        order.CancelItem(productId);

        // Assert
        var cancelledItem = order.OrderItems.First();
        cancelledItem.Cancelled.Should().BeTrue();
    }

    [Fact]
    public void CancelItem_ShouldNotThrow_WhenProductDoesNotExist()
    {
        // Arrange
        var order = Order.Create(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow);
        var nonExistingProductId = _faker.Random.Guid();

        // Act
        Action act = () => order.CancelItem(nonExistingProductId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CalculateOrderAmount_ShouldCalculateCorrectAmounts()
    {
        // Arrange
        var order = Order.Create(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow, discount: 20);
        var activeProduct1 = _faker.Random.Guid();
        var activeProduct2 = _faker.Random.Guid();
        var cancelledProduct = _faker.Random.Guid();

        order.AddItem(activeProduct1, 2, 10);
        order.AddItem(activeProduct2, 3, 15);
        order.AddItem(cancelledProduct, 1, 100);

        // Act
        order.CancelItem(cancelledProduct);

        // Assert
        order.TotalAmount.Should().Be(45);
        order.CancelledItemsAmount.Should().Be(100);
    }
}