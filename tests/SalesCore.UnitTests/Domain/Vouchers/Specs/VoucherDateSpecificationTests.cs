using Bogus;
using FluentAssertions;
using SalesCore.Domain.Vouchers;
using SalesCore.Domain.Vouchers.Specs;

namespace SalesCore.UnitTests.Domain.Vouchers.Specs;

public class VoucherSpecsTests
{
    private readonly Faker<Voucher> _faker;

    public VoucherSpecsTests()
    {
        _faker = new Faker<Voucher>()
            .CustomInstantiator(f => (Voucher)Activator.CreateInstance(typeof(Voucher), true)!)
            .RuleFor(v => v.ExpirationDate, f => f.Date.Future())
            .RuleFor(v => v.Quantity, f => f.Random.Int(0, 100))
            .RuleFor(v => v.Active, f => f.Random.Bool())
            .RuleFor(v => v.Used, f => f.Random.Bool());
    }

    [Fact]
    public void VoucherDateSpecification_ShouldReturnTrue_WhenVoucherIsNotExpired()
    {
        // Arrange
        var voucher = _faker.Clone().RuleFor(v => v.ExpirationDate, f => f.Date.Future()).Generate();
        var spec = new VoucherDateSpecification();

        // Act
        var result = spec.ToExpression().Compile()(voucher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VoucherDateSpecification_ShouldReturnFalse_WhenVoucherIsExpired()
    {
        // Arrange
        var voucher = _faker.Clone().RuleFor(v => v.ExpirationDate, f => f.Date.Past()).Generate();
        var spec = new VoucherDateSpecification();

        // Act
        var result = spec.ToExpression().Compile()(voucher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VoucherQuantitySpecification_ShouldReturnTrue_WhenQuantityIsGreaterThanZero()
    {
        // Arrange
        var voucher = _faker.Clone().RuleFor(v => v.Quantity, f => f.Random.Int(1, 100)).Generate();
        var spec = new VoucherQuantitySpecification();

        // Act
        var result = spec.ToExpression().Compile()(voucher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VoucherQuantitySpecification_ShouldReturnFalse_WhenQuantityIsZero()
    {
        // Arrange
        var voucher = _faker.Clone().RuleFor(v => v.Quantity, 0).Generate();
        var spec = new VoucherQuantitySpecification();

        // Act
        var result = spec.ToExpression().Compile()(voucher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VoucherActiveSpecification_ShouldReturnTrue_WhenVoucherIsActiveAndNotUsed()
    {
        // Arrange
        var voucher = _faker.Clone().RuleFor(v => v.Active, true).RuleFor(v => v.Used, false).Generate();
        var spec = new VoucherActiveSpecification();

        // Act
        var result = spec.ToExpression().Compile()(voucher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VoucherActiveSpecification_ShouldReturnFalse_WhenVoucherIsNotActiveOrUsed()
    {
        // Arrange
        var voucher = _faker.Clone().RuleFor(v => v.Active, false).Generate();
        var spec = new VoucherActiveSpecification();

        // Act
        var result = spec.ToExpression().Compile()(voucher);

        // Assert
        result.Should().BeFalse();
    }
}