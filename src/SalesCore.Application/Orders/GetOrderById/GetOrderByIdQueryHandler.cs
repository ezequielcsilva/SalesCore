using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Orders;

namespace SalesCore.Application.Orders.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResult>
{
    public async Task<Result<GetOrderByIdResult>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<GetOrderByIdResult>(OrderErrors.NotFound);

        var result = new GetOrderByIdResult(
            order.Id,
            order.DateAdded,
            order.CustomerId,
            order.BranchId,
            order.TotalAmount,
            order.Discount,
            order.CancelledItemsAmount,
            order.OrderItems.Select(oi =>
                new GetOrderByIdItemsResult(
                    oi.Id,
                    oi.ProductId,
                    oi.Quantity,
                    oi.Price,
                    oi.GetAmount(),
                    oi.Cancelled)),
            order.OrderStatus);

        return Result.Success(result);
    }
}