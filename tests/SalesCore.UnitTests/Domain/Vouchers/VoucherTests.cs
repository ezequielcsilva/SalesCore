using FluentAssertions;
using SalesCore.Domain.Vouchers;

namespace SalesCore.UnitTests.Domain.Vouchers;

public class VoucherTests
{
    [Fact]
    public void Create_ShouldCreateValidVoucher_WhenValidDataIsProvided()
    {
        // Arrange
        var code = "VOUCHER2024";
        decimal? percentage = 10;
        decimal? discount = null;
        int quantity = 5;
        var discountType = VoucherDiscountType.Percentage;
        var expirationDate = DateTime.Now.AddDays(30);

        // Act
        var voucher = Voucher.Create(code, percentage, discount, quantity, discountType, expirationDate);

        // Assert
        voucher.Code.Should().Be(code);
        voucher.Percentage.Should().Be(percentage);
        voucher.Discount.Should().BeNull();
        voucher.Quantity.Should().Be(quantity);
        voucher.DiscountType.Should().Be(discountType);
        voucher.ExpirationDate.Should().Be(expirationDate);
        voucher.Active.Should().BeTrue();
        voucher.Used.Should().BeFalse();
        voucher.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CanUse_ShouldReturnTrue_WhenVoucherIsActiveNotExpiredAndHasQuantity()
    {
        // Arrange
        var voucher = Voucher.Create("VOUCHER2024", 10, null, 5, VoucherDiscountType.Percentage, DateTime.Now.AddDays(10));

        // Act
        var canUse = voucher.CanUse();

        // Assert
        canUse.Should().BeTrue();
    }

    [Fact]
    public void CanUse_ShouldReturnFalse_WhenVoucherIsExpired()
    {
        // Arrange
        var voucher = Voucher.Create("EXPIREDVOUCHER", 10, null, 5, VoucherDiscountType.Percentage, DateTime.Now.AddDays(-1));

        // Act
        var canUse = voucher.CanUse();

        // Assert
        canUse.Should().BeFalse();
    }

    [Fact]
    public void CanUse_ShouldReturnFalse_WhenVoucherHasNoQuantity()
    {
        // Arrange
        var voucher = Voucher.Create("NOSTOCKVOUCHER", 10, null, 0, VoucherDiscountType.Percentage, DateTime.Now.AddDays(10));

        // Act
        var canUse = voucher.CanUse();

        // Assert
        canUse.Should().BeFalse();
    }

    [Fact]
    public void SetAsUsed_ShouldMarkVoucherAsUsed()
    {
        // Arrange
        var voucher = Voucher.Create("USEDVOUCHER", 10, null, 5, VoucherDiscountType.Percentage, DateTime.Now.AddDays(10));

        // Act
        voucher.SetAsUsed();

        // Assert
        voucher.Active.Should().BeFalse();
        voucher.Used.Should().BeTrue();
        voucher.Quantity.Should().Be(0);
        voucher.UsedAt.Should().NotBeNull();
        voucher.UsedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetOne_ShouldReduceQuantityAndSetAsUsedWhenQuantityIsZero()
    {
        // Arrange
        var voucher = Voucher.Create("VOUCHER2024", 10, null, 1, VoucherDiscountType.Percentage, DateTime.Now.AddDays(10));

        // Act
        voucher.GetOne();

        // Assert
        voucher.Quantity.Should().Be(0);
        voucher.Active.Should().BeFalse();
        voucher.Used.Should().BeTrue();
    }

    [Fact]
    public void GetOne_ShouldReduceQuantityButNotSetAsUsed_WhenQuantityGreaterThanOne()
    {
        // Arrange
        var voucher = Voucher.Create("VOUCHER2024", 10, null, 3, VoucherDiscountType.Percentage, DateTime.Now.AddDays(10));

        // Act
        voucher.GetOne();

        // Assert
        voucher.Quantity.Should().Be(2);
        voucher.Active.Should().BeTrue();
        voucher.Used.Should().BeFalse();
    }
}