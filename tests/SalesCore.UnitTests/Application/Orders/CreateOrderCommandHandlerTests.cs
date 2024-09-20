using Bogus;
using FluentAssertions;
using NSubstitute;
using SalesCore.Application.Abstractions.Clock;
using SalesCore.Application.Orders.CreateOrder;
using SalesCore.Domain.Orders;
using SalesCore.Domain.Vouchers;

namespace SalesCore.UnitTests.Application.Orders;

public class CreateOrderCommandHandlerTests
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly CreateOrderCommandHandler _handler;
    private readonly Faker _faker;

    public CreateOrderCommandHandlerTests()
    {
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _voucherRepository = Substitute.For<IVoucherRepository>();
        _orderRepository = Substitute.For<IOrderRepository>();
        _handler = new CreateOrderCommandHandler(_dateTimeProvider, _voucherRepository, _orderRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenOrderIsValid()
    {
        // Arrange
        var orderItems = new List<CreateOrderItemRequest>
            {
                new(_faker.Random.Guid(), _faker.Random.Int(1, 10),
                    _faker.Random.Decimal(1, 100))
            };
        var request = new CreateOrderCommand(new CreateOrderRequest(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
            orderItems.Select(oi => oi.Quantity * oi.Price).Sum(),
            orderItems,
            string.Empty,
            false,
            0));

        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
        _voucherRepository.GetVoucherByCode(Arg.Any<string>()).Returns((Voucher?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepository.Received(1).Add(Arg.Is<Order>(o => o.OrderItems.Count == 1));
    }

    [Fact]
    public async Task Handle_ShouldReturnVoucherNotFound_WhenVoucherIsInvalid()
    {
        // Arrange
        var orderItems = new List<CreateOrderItemRequest>
            {
                new(_faker.Random.Guid(), _faker.Random.Int(1, 10),
                    _faker.Random.Decimal(1, 100))
            };
        var request = new CreateOrderCommand(new CreateOrderRequest(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
            orderItems.Select(oi => oi.Quantity * oi.Price).Sum(),
            orderItems,
            "INVALID_VOUCHER",
            true,
            0));

        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
        _voucherRepository.GetVoucherByCode("INVALID_VOUCHER").Returns((Voucher?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(VoucherErrors.VoucherNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenVoucherIsExpired()
    {
        // Arrange
        var expiredVoucher = Voucher.Create("EXPIRED_VOUCHER", 20, 100, 1, VoucherDiscountType.Percentage,
            DateTime.UtcNow.AddDays(-1));
        var orderItems = new List<CreateOrderItemRequest>
            {
                new(_faker.Random.Guid(), _faker.Random.Int(1, 10),
                    _faker.Random.Decimal(1, 100))
            };
        var request = new CreateOrderCommand(new CreateOrderRequest(
            _faker.Random.Guid(),
            _faker.Random.Guid(),
            orderItems.Select(oi => oi.Quantity * oi.Price).Sum(),
            orderItems,
            "EXPIRED_VOUCHER",
            true,
            0));

        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
        _voucherRepository.GetVoucherByCode("EXPIRED_VOUCHER").Returns(expiredVoucher);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(VoucherErrors.Expired);
    }
}