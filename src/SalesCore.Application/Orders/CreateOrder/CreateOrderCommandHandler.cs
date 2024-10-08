﻿using SalesCore.Application.Abstractions.Clock;
using SalesCore.Application.Abstractions.Data;
using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Orders;
using SalesCore.Domain.Vouchers;
using SalesCore.Domain.Vouchers.Specs;

namespace SalesCore.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    IDateTimeProvider dateTimeProvider, 
    IVoucherRepository voucherRepository, 
    IOrderRepository orderRepository, 
    IDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<Result<CreateOrderResult>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = MapOrder(request);

        var applyVoucherResult = await ApplyVoucher(request, order);

        if (applyVoucherResult.IsFailure)
            return Result.Failure<CreateOrderResult>(applyVoucherResult.Errors);

        var validateOrderResult = ValidateOrderTotalAmount(request, order);

        if (validateOrderResult.IsFailure)
            return Result.Failure<CreateOrderResult>(validateOrderResult.Errors);

        orderRepository.Add(order);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id);
    }

    private Order MapOrder(CreateOrderCommand request)
    {
        var order = Order.Create(request.Order.CustomerId, request.Order.BranchId, dateTimeProvider.UtcNow, request.Order.HasVoucher, request.Order.Discount);

        foreach (var orderItem in request.Order.OrderItems)
        {
            order.AddItem(orderItem.ProductId, orderItem.Quantity, orderItem.Price);
        }

        return order;
    }

    private async Task<Result> ApplyVoucher(CreateOrderCommand request, Order order)
    {
        if (!request.Order.HasVoucher) return Result.Success();

        var voucher = await voucherRepository.GetVoucherByCodeAsync(request.Order.Voucher);

        if (voucher is null)
            return Result.Failure(VoucherErrors.VoucherNotFound);

        var voucherValidation = VoucherValidation.Validate(voucher);

        if (voucherValidation.Any())
            return Result.Failure(voucherValidation);

        order.AssociateVoucher(voucher);
        voucher.GetOne();

        voucherRepository.Update(voucher);

        return Result.Success();
    }

    private static Result ValidateOrderTotalAmount(CreateOrderCommand request, Order order)
    {
        var expectedAmountAfterDiscount = request.Order.Amount - request.Order.Discount;

        if (order.TotalAmount != expectedAmountAfterDiscount)
            return Result.Failure(OrderErrors.TotalAmountMismatch);

        return order.Discount != request.Order.Discount ? Result.Failure(OrderErrors.SentAmountMismatch) : Result.Success();
    }
}