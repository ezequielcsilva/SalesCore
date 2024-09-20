using FluentAssertions;
using SalesCore.Domain.Orders;
using SalesCore.Domain.Vouchers;

namespace SalesCore.UnitTests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void Create_ShouldInstantiateOrderCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var dateAdded = DateTime.UtcNow;

        // Act
        var order = Order.Create(customerId, branchId, dateAdded);

        // Assert
        order.Should().NotBeNull();
        order.CustomerId.Should().Be(customerId);
        order.BranchId.Should().Be(branchId);
        order.DateAdded.Should().Be(dateAdded);
        order.TotalAmount.Should().Be(0);
        order.HasVoucher.Should().BeFalse();
    }

    [Fact]
    public void AddItem_ShouldIncreaseTotalAmount()
    {
        // Arrange
        var order = CreateSampleOrder();
        var productId = Guid.NewGuid();
        var quantity = 2;
        var price = 50m;

        // Act
        order.AddItem(productId, quantity, price);

        // Assert
        order.TotalAmount.Should().Be(100m);
        order.OrderItems.Count.Should().Be(1);
    }

    [Fact]
    public void AddItem_ShouldUpdateExistingItemQuantityAndPrice()
    {
        // Arrange
        var order = CreateSampleOrder();
        var productId = Guid.NewGuid();
        var initialQuantity = 1;
        var initialPrice = 20m;
        order.AddItem(productId, initialQuantity, initialPrice);

        // Act
        var updatedQuantity = 3;
        var updatedPrice = 30m;
        order.AddItem(productId, updatedQuantity, updatedPrice);

        // Assert
        var item = order.OrderItems.First(x => x.ProductId == productId);
        item.Quantity.Should().Be(updatedQuantity);
        item.Price.Should().Be(updatedPrice);
        order.TotalAmount.Should().Be(90m);
    }

    [Fact]
    public void CancelItem_ShouldUpdateCancelledItemsAmount()
    {
        // Arrange
        var order = CreateSampleOrder();
        var productId = Guid.NewGuid();
        order.AddItem(productId, 2, 50m);

        // Act
        order.CancelItem(productId);

        // Assert
        order.TotalAmount.Should().Be(0);
        order.CancelledItemsAmount.Should().Be(100m);
    }

    [Fact]
    public void AssociateVoucher_ShouldApplyDiscountCorrectly()
    {
        // Arrange
        var order = CreateSampleOrder();
        var voucher = Voucher.Create("VOUCHER123", percentage: 10, discount: null, quantity: 1,
                                     VoucherDiscountType.Percentage, DateTime.UtcNow.AddDays(1));

        order.AddItem(Guid.NewGuid(), 2, 100m);

        // Act
        order.AssociateVoucher(voucher);

        // Assert
        order.HasVoucher.Should().BeTrue();
        order.TotalAmount.Should().Be(180m);
        order.Discount.Should().Be(20m);
    }

    [Fact]
    public void AssociateVoucher_WithFixedDiscount_ShouldApplyDiscountCorrectly()
    {
        // Arrange
        var order = CreateSampleOrder();
        var voucher = Voucher.Create("VOUCHER123", percentage: null, discount: 50m, quantity: 1,
                                     VoucherDiscountType.Value, DateTime.UtcNow.AddDays(1));

        order.AddItem(Guid.NewGuid(), 2, 100m);

        // Act
        order.AssociateVoucher(voucher);

        // Assert
        order.HasVoucher.Should().BeTrue();
        order.TotalAmount.Should().Be(150m);
        order.Discount.Should().Be(50m);
    }

    [Fact]
    public void RecalculateOrderAmounts_ShouldNotApplyDiscount_WhenVoucherIsNotPresent()
    {
        // Arrange
        var order = CreateSampleOrder();
        order.AddItem(Guid.NewGuid(), 2, 100m);

        // Act
        order.RecalculateOrderAmounts();

        // Assert
        order.TotalAmount.Should().Be(200m);
        order.Discount.Should().Be(0);
    }

    private static Order CreateSampleOrder()
    {
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var dateAdded = DateTime.UtcNow;
        return Order.Create(customerId, branchId, dateAdded);
    }
}